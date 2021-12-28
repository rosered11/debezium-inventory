using Confluent.Kafka;
using InventoryWorker.Tests.Utilities;
using Microsoft.Extensions.Configuration;
using Snapshooter.Xunit;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace InventoryWorker.Tests.Process
{
    public class GeneratePayloadTest
    {
        private const string payload = @"{
            ""payload"": ""{ \""receiveOrderNumber\"": \""RC-211100005\"", \""receiveOrderDetails\"": [ { \""qty\"": 150, \""uom\"": \""Unit\"", \""partNo\"": \""partNo111111\"" }, { \""qty\"": 300, \""uom\"": \""Unit\"", \""partNo\"": \""partNo333333\"" } ] }""
          }";
        private InventoryWorker.Process CreateProcessService(IConfiguration config) => new InventoryWorker.Process(config, null);

        [Fact]
        public void WhenCallGeneratePayloadViaInputOutbox_ShouldReturnPayloadIsCorrect()
        {
            Dictionary<string, string> configMapping = new Dictionary<string, string>();
            configMapping.Add("WarehouseLocationNo", "wh");
            var config = ConfigurationTestBuilder.Build(configMapping);
            var header = new Headers();
            header.Add("type", Encoding.UTF8.GetBytes("abcd"));
            var message = new Message<Ignore, string>();
            message.Headers = header;
            message.Value = payload;

            var input = new ConsumeResult<Ignore, string>();
            input.Message = message;

            var service = CreateProcessService(config);

            var result = service.GeneratePayload(input);

            Snapshot.Match(result);
        }
    }
}
