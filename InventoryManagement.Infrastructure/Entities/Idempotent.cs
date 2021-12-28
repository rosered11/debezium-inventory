using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Infrastructure.Entities
{
    [Table("idempotent")]
    public class Idempotent
    {
        public string EventId { get; set; }
        public string EventType { get; set; }
    }
}
