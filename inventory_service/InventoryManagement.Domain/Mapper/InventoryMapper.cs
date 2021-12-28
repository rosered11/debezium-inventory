using Boxed.Mapping;

namespace InventoryManagement.Domain.Mapper
{
    public class InventoryToInventoryRowResponseMapper : IMapper<Infrastructure.Entities.Inventory, DTO.Responses.Row>
    {
        public void Map(Infrastructure.Entities.Inventory source, DTO.Responses.Row destination)
        {
            destination.PartNo = source.PartNo;
            destination.PoQuantity = source.POQty;
            destination.ReceivingQuantity = source.ReceivingQty;
            destination.BalanceQuantity = source.BalanceQty;
            destination.RequestQuantity = source.RequestQty;
            destination.AvailableQuantity = source.AvailableQty;
            destination.Uom = source.Uom;
        }
    }
}
