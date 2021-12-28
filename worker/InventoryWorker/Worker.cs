using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InventoryWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly Process _process;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, Process process)
        {
            _logger = logger;
            _configuration = configuration;
            _process = process;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumerConfig = new ConsumerConfig
            {
                GroupId = _configuration["GroupId"],
                BootstrapServers = _configuration["Hosts"],
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoOffsetStore = false
            };

            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
            {
                int currentConsumerCount = 0;
                int currentProcessCount = 0;
                consumer.Subscribe(_configuration["Topic"]);
                while (!stoppingToken.IsCancellationRequested)
                {
                    ConsumeResult<Ignore, string> consumeResult;

                    // Check consume message from message queue
                    try
                    {
                        consumeResult = consumer.Consume(stoppingToken);
                        currentConsumerCount = 0;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Cann't consume message from bus.");
                        currentConsumerCount++;
                        if(currentConsumerCount >= _configuration.GetValue<int>("Consumer:Retry:Max"))
                        {
                            throw new OperationCanceledException($"Cann't consume message from bus, please check configuration and run the service agian.");
                        }
                        continue;
                    }

                    // Process the message
                    try
                    {
                        _logger.LogInformation($"Topic: {consumeResult.Topic}");
                        _logger.LogInformation($"Partition: {consumeResult.Partition.Value}");
                        _logger.LogInformation($"Offset: {consumeResult.Offset.Value}");

                        await _process.Run(consumeResult, stoppingToken);
                        currentProcessCount = 0;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Cann't process message from bus.");

                        currentProcessCount++;
                        if (currentProcessCount >= _configuration.GetValue<int>("Process:Retry:Max"))
                        {
                            Thread.Sleep(10000);
                            currentProcessCount = 0;
                        }

                        consumer.Assign(consumeResult.TopicPartitionOffset);
                        continue;
                        //throw new OperationCanceledException($"Cann't process message from bus, please check configuration and run the service agian.");
                    }

                    // Commit message for used
                    try
                    {
                        consumer.StoreOffset(consumeResult);
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, $"Cann't commit message queue for topic: {consumeResult.Topic}, partition: {consumeResult.Partition.Value}, offset: {consumeResult.Offset.Value}");
                        continue;
                    }
                }
            }
        }
    }
}
