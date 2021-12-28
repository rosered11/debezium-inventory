namespace InventoryManagement.Domain.DTO.Models
{
    public class PartModel
    {
        public string PartName { get; set; }
        public string PartNo { get; set; }
    }

    public class PartNoWithAvailable
    {
        public string PartNo { get; set; }
        public int Available { get; set; }
    }
}
