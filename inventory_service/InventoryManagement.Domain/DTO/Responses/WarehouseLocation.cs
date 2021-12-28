using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace InventoryManagement.Domain.DTO.Responses
{
    public class WarehouseLocation : BaseResponse
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

        public class Row
        {
            [JsonPropertyName("code")]
            public string Code { get; set; }
            [JsonPropertyName("description")]
            public string Description { get; set; }
            [JsonPropertyName("address")]
            public string Address { get; set; }
        }
    }
}
