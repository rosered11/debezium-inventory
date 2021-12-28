using System;
using System.Net.Http;
using Inventory.Worker.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace InventoryWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // Client Masterdata
                    services.AddHttpClient<IInventoryClient, InventoryClient>(client =>
                    {
                        client.BaseAddress = new Uri(hostContext.Configuration["External:Inventory:BaseUrl"]);
                    })
                    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                    {
                        ClientCertificateOptions = ClientCertificateOption.Manual,
                        ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
                    })
                    .AddPolicyHandler(RetryPolicy.GetRetryPolicy(hostContext.Configuration))
                    .AddPolicyHandler(CircuitBreakerPolicy.GetCircuitBreakerPolicy(hostContext.Configuration));
                    services.AddSingleton<Process>();

                    services.AddHostedService<Worker>();
                });
    }
}
