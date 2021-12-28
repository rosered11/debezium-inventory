using InventoryManagement.Domain.DTO.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InventoryManagement.Domain.DTO.Requests
{
    public class Inventory
    {
        [Required]
        [JsonPropertyName("partNo")]
        public string PartNo { get; set; }
        [Required]
        [JsonPropertyName("warehouseLocationNo")]
        public string WarehouseLocationNo { get; set; }
        [Required]
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
        [ValidateEnum]
        [Required]
        [JsonPropertyName("transactionType")]
        public string TransactionType { get; set; }
        [Required]
        [JsonPropertyName("uom")]
        public string Uom { get; set; }
    }

    public class InventoryRequest
    {
        [Required]
        [JsonPropertyName("requestOrderNo")]
        public string RequestOrderNo { get; set; }
        [Required]
        [JsonPropertyName("warehouseLocationNo")]
        public string WarehouseLocationNo { get; set; }
        
        [Required]
        [ValidateList]
        [JsonPropertyName("parts")]
        public IEnumerable<Part> Parts { get; set; }
        
    }

    public class Part
    {
        [Required]
        [JsonPropertyName("no")]
        public string No { get; set; }
        [Required]
        [JsonPropertyName("qty")]
        public int Qty { get; set; }
        [Required]
        [JsonPropertyName("uom")]
        public string Uom { get; set; }
    }

    public class InventoryEventBase
    {
        [Required]
        [JsonPropertyName("warehouseLocationNo")]
        public string WarehouseLocationNo { get; set; }
        [Required]
        [ValidateList]
        [JsonPropertyName("parts")]
        public IEnumerable<Part> Parts { get; set; }
    }
    public class ReceiveEvent : InventoryEventBase
    {
        [Required]
        [JsonPropertyName("receiveOrderNo")]
        public string ReceiveOrderNo { get; set; }
        [Required]
        [ValidateEnum(nameof(ValidateEnumType.Receive))]
        [JsonPropertyName("eventType")]
        public string EventType { get; set; }
    }

    public class RequestEvent : InventoryEventBase
    {
        [Required]
        [JsonPropertyName("requestOrderNo")]
        public string RequestOrderNo { get; set; }
        [Required]
        [ValidateEnum(nameof(ValidateEnumType.Request))]
        [JsonPropertyName("eventType")]
        public string EventType { get; set; }
    }
}
