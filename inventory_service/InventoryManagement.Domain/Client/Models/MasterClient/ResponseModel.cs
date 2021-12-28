using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace InventoryManagement.Domain.Client.Models.MasterClient
{
    #region Response Part model
    public class PartModel
    {
        [JsonPropertyName("data")]
        public IEnumerable<DataModel> Data { get; set; }
        public class DataModel
        {
            [JsonPropertyName("no")]
            public string No { get; set; }
            [JsonPropertyName("name")]
            public string Name { get; set; }
            [JsonPropertyName("uom_id")]
            public UomModel UomId { get; set; }
        }

        public class UomModel
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }
    }
    
    #endregion

    #region Response Warehouse mode
    public class WarehouseLocationModel
    {
        [JsonPropertyName("data")]
        public IEnumerable<DataModel> Data { get; set; }
        public class DataModel
        {
            [JsonPropertyName("code")]
            public string Code { get; set; }
            [JsonPropertyName("name")]
            public string Name { get; set; }
            [JsonPropertyName("description")]
            public string Description { get; set; }
        }
    }

    #endregion
}
