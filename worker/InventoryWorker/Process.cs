using Confluent.Kafka;
using Inventory.Worker.Client;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryWorker
{
    public class Process
    {
        private readonly IConfiguration _configuration;
        private readonly IInventoryClient _inventoryClient;

        public Process(IConfiguration configuration, IInventoryClient inventoryClient)
        {
            _configuration = configuration;
            _inventoryClient = inventoryClient;
        }


        public async Task Run(ConsumeResult<Ignore, string> consumeResult, CancellationToken stoppingToken)
        {
            var idByte = consumeResult.Message.Headers.GetLastBytes("id");
            var eventId = Encoding.UTF8.GetString(idByte);

            string payload = GeneratePayload(consumeResult);

            await _inventoryClient.PostReceiveEvent(payload, eventId, stoppingToken);
        }

        internal string GeneratePayload(ConsumeResult<Ignore, string> consumeResult)
        {
            var typeByte = consumeResult.Message.Headers.GetLastBytes("type");
            var type = Encoding.UTF8.GetString(typeByte);

            JsonWriterOptions writerOptions = new() { Indented = true, };
            using (MemoryStream stream = new())
            using (Utf8JsonWriter writer = new(stream, writerOptions))
            using (JsonDocument document = JsonDocument.Parse(consumeResult.Message.Value))
            {
                var root = document.RootElement;
                var payload = JsonSerializer.Deserialize<JsonElement>(root.GetProperty("payload").GetString());
                var receiveOrder = payload.GetProperty("receiveOrderNumber").GetString();
                var parts = payload.GetProperty("receiveOrderDetails").EnumerateArray();


                // Write json
                writer.WriteStartObject();
                writer.WritePropertyName("receiveOrderNo");
                writer.WriteStringValue(receiveOrder);
                writer.WriteString("warehouseLocationNo", _configuration.GetValue<string>("WarehouseLocationNo"));
                writer.WriteString("eventType", type);
                writer.WriteStartArray("parts");
                foreach (var part in parts)
                {
                    writer.WriteStartObject();
                    var partNo = part.GetProperty("partNo").GetString();
                    writer.WriteString("no", partNo);
                    var qty = part.GetProperty("qty").GetInt32();
                    writer.WriteNumber("qty", qty);
                    var uom = part.GetProperty("uom").GetString();
                    writer.WriteString("uom", uom);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
                writer.WriteEndObject();
                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }
}
