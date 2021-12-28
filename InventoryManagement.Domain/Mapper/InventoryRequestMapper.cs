using Boxed.Mapping;

namespace InventoryManagement.Domain.Mapper
{
    public class InvetoryRequestPartToInventoryRequestMapper : IMapper<DTO.Requests.Part, DTO.Requests.Inventory>
    {
        public void Map(DTO.Requests.Part source, DTO.Requests.Inventory destination)
        {
            destination.PartNo = source.No;
            destination.Quantity = source.Qty;
            destination.Uom = source.Uom;
        }
    }
}
