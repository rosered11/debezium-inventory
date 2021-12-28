using Boxed.Mapping;
using InventoryManagement.Domain.DTO.Models;

namespace InventoryManagement.Domain.Mapper
{
    public class AvailableOfPathModelMapper : IMapper<AvailableOfPathModel, DTO.Responses.BadRequestForRequestValidationResponse.PartModel>
    {
        public void Map(AvailableOfPathModel source, DTO.Responses.BadRequestForRequestValidationResponse.PartModel destination)
        {
            destination.Uom = source.Uom;
            destination.No = source.PartNo;
            destination.Qty = source.Qty;
            destination.IsEnough = source.IsEnough;
            destination.Availiable = source.AvailableQuantity;
        }
    }
}
