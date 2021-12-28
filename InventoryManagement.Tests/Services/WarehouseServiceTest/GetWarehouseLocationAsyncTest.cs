using InventoryManagement.Domain.Client;
using InventoryManagement.Domain.Client.Models.MasterClient;
using InventoryManagement.Domain.Services;
using InventoryManagement.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;
using Sieve.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace InventoryManagement.Tests.Services.WarehouseServiceTest
{
    public class GetWarehouseLocationAsyncTest
    {
        private WarehouseService CreateWarehouseService(UnitOfWork unitOfWork, IMasterClient masterClient) => new WarehouseService(unitOfWork, masterClient);

        [Fact]
        public async Task WhenGetWarehouseHasBalance_ShouldReturnThatWarehouse()
        {
            using (var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
            ))
            {
                MockData(context);
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var masterClient = new Mock<IMasterClient>();
                masterClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(default(WarehouseLocationModel)));

                var service = CreateWarehouseService(unitOfWork, masterClient.Object);

                var result = await service.GetWarehouseLocationHasBalanceAsync(new Sieve.Models.SieveModel());

                Assert.Equal(3, result.Result.TotalRows);
                Assert.Equal(3, result.Result.Rows.Count());
                Assert.Single(result.Result.Rows.Where(x => x.Code == "w2"));
            };
        }

        [Fact]
        public async Task WhenGetWarehouseHasNotBalance_ShouldNotReturnThatWarehouse()
        {
            using (var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
            ))
            {
                MockData(context);
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var masterClient = new Mock<IMasterClient>();
                masterClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(default(WarehouseLocationModel)));

                var service = CreateWarehouseService(unitOfWork, masterClient.Object);

                var result = await service.GetWarehouseLocationHasBalanceAsync(new Sieve.Models.SieveModel());

                Assert.Empty(result.Result.Rows.Where(x => x.Code == "w1"));
            };
        }

        [Fact]
        public async Task WhenGetWarehouseHasBalanceSomeOne_ShouldReturnThatWarehouse()
        {
            using (var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
            ))
            {
                MockData(context);
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var masterClient = new Mock<IMasterClient>();
                masterClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(default(WarehouseLocationModel)));

                var service = CreateWarehouseService(unitOfWork, masterClient.Object);

                var result = await service.GetWarehouseLocationHasBalanceAsync(new Sieve.Models.SieveModel());

                Assert.Single(result.Result.Rows.Where(x => x.Code == "w3"));
            };
        }

        [Fact]
        public async Task WhenGetWarehouseHaveBalance_ShouldReturnSingleWarehouse()
        {
            using (var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
            ))
            {
                MockData(context);
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var masterClient = new Mock<IMasterClient>();
                masterClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(default(WarehouseLocationModel)));

                var service = CreateWarehouseService(unitOfWork, masterClient.Object);

                var result = await service.GetWarehouseLocationHasBalanceAsync(new Sieve.Models.SieveModel());

                Assert.Single(result.Result.Rows.Where(x => x.Code == "w4"));
            };
        }

        [Fact]
        public async Task WhenGetWarehouse_ButWarehouseNotFound_ShouldReturnEmpty()
        {
            using (var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
            ))
            {
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var masterClient = new Mock<IMasterClient>();
                masterClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(default(WarehouseLocationModel)));

                var service = CreateWarehouseService(unitOfWork, masterClient.Object);

                var result = await service.GetWarehouseLocationHasBalanceAsync(new Sieve.Models.SieveModel());

                Assert.Empty(result.Result.Rows);
                Assert.Equal(0, result.Result.TotalRows);
            };
        }

        private void MockData(ApplicationDbContext context)
        {
            context.Inventories.AddRange(
                new Infrastructure.Entities.Inventory { 
                    WarehouseLocationNo = "w1",
                    PartNo = "p1",
                    BalanceQty = 0
                },
                new Infrastructure.Entities.Inventory
                {
                    WarehouseLocationNo = "w2",
                    PartNo = "p1",
                    BalanceQty = 10
                },
                new Infrastructure.Entities.Inventory
                {
                    WarehouseLocationNo = "w3",
                    PartNo = "p1",
                    BalanceQty = 0
                },
                new Infrastructure.Entities.Inventory
                {
                    WarehouseLocationNo = "w3",
                    PartNo = "p2",
                    BalanceQty = 5
                },
                new Infrastructure.Entities.Inventory
                {
                    WarehouseLocationNo = "w4",
                    PartNo = "p1",
                    BalanceQty = 2
                },
                new Infrastructure.Entities.Inventory
                {
                    WarehouseLocationNo = "w4",
                    PartNo = "p2",
                    BalanceQty = 5
                }
            );
            context.SaveChanges();
        }
    }
}
