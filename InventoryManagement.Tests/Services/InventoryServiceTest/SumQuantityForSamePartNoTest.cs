using InventoryManagement.Domain.Services;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace InventoryManagement.Tests.Services.InventoryServiceTest
{
    public class SumQuantityForSamePartNoTest
    {
        private static InventoryService CreateInventoryService()
            => new InventoryService(null, null, null, null, null, null);


        [Fact]
        public void WhenRequestPartsHaveSamePartNoInListItem_ShouldSumQuantityForSamePartNoAndGrouppingToOneItem()
        {
            var requestParts = new List<Domain.DTO.Requests.Part>
            {
                new Domain.DTO.Requests.Part
                {
                    No = "partNo1",
                    Qty = 10
                },
                new Domain.DTO.Requests.Part
                {
                    No = "partNo1",
                    Qty = 10
                },
                new Domain.DTO.Requests.Part
                {
                    No = "partNo2",
                    Qty = 10
                }
            };
            var service = CreateInventoryService();

            var result = service.SumQuantityForSamePartNo(requestParts);

            Assert.Equal(2, result.Count());
            Assert.Equal(20, result.Single(x => x.No == "partNo1").Qty);
            Assert.Equal(10, result.Single(x => x.No == "partNo2").Qty);
        }

        [Fact]
        public void WhenRequestPartsDontHaveSamePartNoInListItem_ShouldReturnSameListItem()
        {
            var requestParts = new List<Domain.DTO.Requests.Part>
            {
                new Domain.DTO.Requests.Part
                {
                    No = "partNo1",
                    Qty = 10
                },
                new Domain.DTO.Requests.Part
                {
                    No = "partNo2",
                    Qty = 5
                },
                new Domain.DTO.Requests.Part
                {
                    No = "partNo3",
                    Qty = 15
                }
            };
            var service = CreateInventoryService();

            var result = service.SumQuantityForSamePartNo(requestParts);

            Assert.Equal(3, result.Count());
            Assert.Equal(10, result.Single(x => x.No == "partNo1").Qty);
            Assert.Equal(5, result.Single(x => x.No == "partNo2").Qty);
            Assert.Equal(15, result.Single(x => x.No == "partNo3").Qty);
        }
    }
}
