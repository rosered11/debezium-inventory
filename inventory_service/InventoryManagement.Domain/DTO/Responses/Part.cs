using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace InventoryManagement.Domain.DTO.Responses
{
    public class PartBase : BaseResponse
    {
        public class Row
        {
            [JsonPropertyName("partNo")]
            public string PartNo { get; set; }
            [JsonPropertyName("partName")]
            public string PartName { get; set; }
            [JsonPropertyName("available")]
            public int Available { get; set; }
            [JsonPropertyName("uom")]
            public string Uom { get; set; }
        }
    }

    public class PartAndWarehouseLocation : PartBase
    {
        [JsonPropertyName("result")]
        public ResultModel Result { get; set; } = new ResultModel();
        public class ResultModel
        {
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
            [JsonPropertyName("warehouseCode")]
            public string WarehouseCode { get; set; }
            [JsonPropertyName("warehouseDescription")]
            public string WarehouseDescription { get; set; }
        }
    }

    public class PartOnly : PartBase
    {
        [JsonPropertyName("result")]
        public ResultModel Result { get; set; } = new ResultModel();
        public class ResultModel
        {
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
    }
}
