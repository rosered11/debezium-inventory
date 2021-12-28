using Boxed.Mapping;
using InventoryManagement.Domain.Mapper;
using InventoryManagement.Domain.Services;
using InventoryManagement.Domain.Utilities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace InventoryManagement.Tests.Services.InventoryServiceTest
{
    public class GetInventoryFromRequestPartsTest
    {
        private static InventoryService CreateInventoryService(IMapper<Domain.DTO.Requests.Part, Domain.DTO.Requests.Inventory> mapper)
            => new InventoryService(null, null, null, null, mapper, null);

        [Theory]
        [InlineData(InputTransactionType.RC_ACCEPTED)]
        [InlineData(InputTransactionType.RC_COMPLETED)]
        [InlineData(InputTransactionType.RC_SUBMITTED)]
        [InlineData(InputTransactionType.RQ_DELIVERED)]
        [InlineData(InputTransactionType.RQ_SUBMITTED)]
        public void WhenGetInventoryByRequestPart_ShouldReturnInventoryIsCorrect(InputTransactionType inputType)
        {
            var mapper = new InvetoryRequestPartToInventoryRequestMapper();
            var service = CreateInventoryService(mapper);
            var requestPart = new Domain.DTO.Requests.Part
            {
                No = "partNo",
                Qty = 10,
                Uom = "uom"
            };

            var result = service.GetRequestInventoryFromRequestParts(new List<Domain.DTO.Requests.Part> { requestPart }, inputType, "wh").ToList();

            var requestInventory = result.First(x => x.PartNo == "partNo");
            Assert.Single(result);
            Assert.Equal("partNo", requestInventory.PartNo);
            Assert.Equal(10, requestInventory.Quantity);
            Assert.Equal(inputType.ToString(), requestInventory.TransactionType);
            Assert.Equal("wh", requestInventory.WarehouseLocationNo);
            Assert.Equal("uom", requestInventory.Uom);
        }
    }
}
