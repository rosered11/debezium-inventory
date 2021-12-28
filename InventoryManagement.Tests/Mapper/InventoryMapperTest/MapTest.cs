using Boxed.Mapping;
using InventoryManagement.Domain.Mapper;
using Xunit;

namespace InventoryManagement.Tests.Mapper.InventoryMapperTest
{
    public class MapTest
    {
        [Fact]
        public void WhenMappingEntityInventoryToResponseRow_ShouldReturnIsCorrect()
        {
            IMapper<Infrastructure.Entities.Inventory, Domain.DTO.Responses.Row> service = new InventoryToInventoryRowResponseMapper();
            var input = new Infrastructure.Entities.Inventory
            {
                PartNo = "p1",
                POQty = 1,
                ReceivingQty = 2,
                BalanceQty = 3,
                RequestQty = 4,
                AvailableQty = 5,
                Uom = "uom"
            };

            var result = service.Map(input);

            Assert.Equal("p1", result.PartNo);
            Assert.Equal(1, result.PoQuantity);
            Assert.Equal(2, result.ReceivingQuantity);
            Assert.Equal(3, result.BalanceQuantity);
            Assert.Equal(4, result.RequestQuantity);
            Assert.Equal(5, result.AvailableQuantity);
            Assert.Equal("uom", result.Uom);
        }
    }
}
