using InventoryManagement.Infrastructure.Entities.Base;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Infrastructure.Entities
{
    [Table("inventory")]
    public class Inventory : BaseEntity
    {
        [Sieve(CanFilter = true)]
        public string PartNo { get; set; }
        public string WarehouseLocationNo { get; set; }
        public int POQty { get; set; }
        public int ReceivingQty { get; set; }
        public int BalanceQty { get; set; }
        public int RequestQty { get; set; }
        public int AvailableQty { get; set; }
        public string Uom { get; set; }
        public uint xmin { get; set; }
    }
}
