using System.Text.Json.Serialization;

namespace InventoryManagement.Domain.DTO.Responses
{
    public class BaseResponse
    {
        [JsonPropertyName("statusCode")]
        public string StatusCode { get; set; }
        [JsonPropertyName("statusMessage")]
        public string StatusMessage { get; set; }
    }
}
