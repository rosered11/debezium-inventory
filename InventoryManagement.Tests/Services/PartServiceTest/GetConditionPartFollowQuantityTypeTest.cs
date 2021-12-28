using InventoryManagement.Domain.Services;
using InventoryManagement.Infrastructure.Entities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace InventoryManagement.Tests.Services.PartServiceTest
{
    public class GetConditionPartFollowQuantityTypeTest
    {
        private PartService CreatePartService() => new PartService(null, null);
        private IQueryable<Inventory> Createdata() => new List<Inventory> {
                new Inventory
                {
                    AvailableQty = 1,
                    BalanceQty = 2,
                    PartNo = "p1"
                },
                new Inventory
                {
                    AvailableQty = 1,
                    BalanceQty = 0,
                    PartNo = "p2"
                },
                new Inventory
                {
                    AvailableQty = 0,
                    BalanceQty = 2,
                    PartNo = "p3"
                }
            }.AsQueryable();

        [Fact]
        public void WhenQuantityTypeIsAvailable_ShouldReturnInventoryHaveAvailableQtyOnly()
        {
            var data = Createdata();

            var service = CreatePartService();

            var condition = service.GetConditionPartFollowQuantityType(Domain.Utilities.QuantityTypeEnum.AVAILABLE);

            var result = data.Where(condition);

            Assert.Equal(2, result.Count());
            Assert.NotNull(result.FirstOrDefault(x => x.PartNo == "p1"));
            Assert.NotNull(result.FirstOrDefault(x => x.PartNo == "p2"));
        }

        [Fact]
        public void WhenQuantityTypeIsBalance_ShouldReturnInventoryHaveBalanceQtyOnly()
        {
            var data = Createdata();

            var service = CreatePartService();

            var condition = service.GetConditionPartFollowQuantityType(Domain.Utilities.QuantityTypeEnum.BALANCE);

            var result = data.Where(condition);

            Assert.Equal(2, result.Count());
            Assert.NotNull(result.FirstOrDefault(x => x.PartNo == "p1"));
            Assert.NotNull(result.FirstOrDefault(x => x.PartNo == "p3"));
        }
    }
}
