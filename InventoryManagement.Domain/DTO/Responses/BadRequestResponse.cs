using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace InventoryManagement.Domain.DTO.Responses
{
    public class BadRequestForRequestValidationResponse : BaseResponse
    {
        [JsonPropertyName("result")]
        public ResultModel Result { get; set; } = new ResultModel();
        public class ResultModel
        {
            [JsonPropertyName("parts")]
            public IEnumerable<PartModel> Parts { get; set; }
        }

        public class PartModel
        {
            [JsonPropertyName("no")]
            public string No { get; set; }
            [JsonPropertyName("qty")]
            public int Qty { get; set; }
            [JsonPropertyName("uom")]
            public string Uom { get; set; }
            [JsonPropertyName("availiable")]
            public int Availiable { get; set; }
            [JsonPropertyName("isEnough")]
            public bool IsEnough { get; set; }
        }
    }
}
