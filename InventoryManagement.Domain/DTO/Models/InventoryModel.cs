namespace InventoryManagement.Domain.DTO.Models
{
    public class AvailableOfPathModel
    {
        public string PartNo { get; set; }
        public int AvailableQuantity { get; set; }
        public bool IsEnough { get; set; }
        public string Uom { get; set; }
        public int Qty { get; set; }
    }
}
