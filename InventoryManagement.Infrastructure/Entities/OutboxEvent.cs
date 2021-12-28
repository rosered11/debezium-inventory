using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Infrastructure.Entities
{
    [Table("outboxevent")]
    public class OutboxEvent
    {
        [Key]
        [Required]
        [Column("id", TypeName = "uuid")]
        public Guid Id { get; set; }
        [Required]
        [Column("aggregatetype", TypeName = "character varying(255)")]
        public string AggregateType { get; set; }
        [Required]
        [Column("aggregateid", TypeName = "character varying(255)")]
        public string AggregateId { get; set; }
        [Required]
        [Column("type", TypeName = "character varying(255)")]
        public string Type { get; set; }
        [Required]
        [Column("payload", TypeName = "jsonb")]
        public string Payload { get; set; }
        [Column("timestamp")]
        public DateTimeOffset TimeStamp { get; set; }
    }
}
