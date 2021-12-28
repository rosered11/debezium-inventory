using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace InventoryManagement.Domain.DTO.Responses
{
    public class Inventory : BaseResponse
    {
        [JsonPropertyName("result")]
        public Result Result { get; set; } = new Result();
    }
    public class Result
    {
        [JsonPropertyName("warehouseLocationNo")]
        public string WarehouseLocationNo { get; set; }
        [JsonPropertyName("totalRows")]
        public int TotalRows
        {
            get
            {
                return this?.Rows?.Count() ?? 0;
            }
        }
        [JsonPropertyName("rows")]
        public IEnumerable<Row> Rows { get; set; }
    }

    public class Row
    {
        [JsonPropertyName("partNo")]
        public string PartNo { get; set; }
        [JsonPropertyName("onPO")]
        public int PoQuantity { get; set; }
        [JsonPropertyName("onReceiving")]
        public int ReceivingQuantity { get; set; }
        [JsonPropertyName("balance")]
        public int BalanceQuantity { get; set; }
        [JsonPropertyName("onRequest")]
        public int RequestQuantity { get; set; }
        [JsonPropertyName("available")]
        public int AvailableQuantity { get; set; }
        [JsonPropertyName("partName")]
        public string PartName { get; set; }
        [JsonPropertyName("uom")]
        public string Uom { get; set; }
    }
}
