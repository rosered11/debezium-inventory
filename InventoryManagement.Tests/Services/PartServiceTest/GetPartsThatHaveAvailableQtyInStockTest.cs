using InventoryManagement.Domain.Client;
using InventoryManagement.Domain.Client.Models.MasterClient;
using InventoryManagement.Domain.Services;
using InventoryManagement.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace InventoryManagement.Tests.Services.PartServiceTest
{
    public class GetPartsThatHaveAvailableQtyInStockTest
    {
        private PartService CreatePartService(UnitOfWork unitOfWork, IMasterClient masterClient) => new PartService(unitOfWork, masterClient);
        #region Check warehouse masterdata
        // has warehouse masterdata
        [Fact]
        public async Task WhenHavePartsAndHaveMasterWarehouse_ShouldReturnWarehouseAreCorrect()
        {
            using (var context = new ApplicationDbContext(
                    new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
             ))
            {
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var masterClient = new Mock<IMasterClient>();
                masterClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(default(PartModel)));
                masterClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(new WarehouseLocationModel
                {
                    Data = new List<WarehouseLocationModel.DataModel> {
                        new WarehouseLocationModel.DataModel { Code = "w1", Name = "desc1"  }
                    }
                }));

                var service = CreatePartService(unitOfWork, masterClient.Object);

                var result = await service.GetPartsThatHaveAvailableQtyInStock(new Sieve.Models.SieveModel(), "w1");

                Assert.Equal("w1", result.Result.WarehouseCode);
                Assert.Equal("desc1", result.Result.WarehouseDescription);
            }
        }

        // warehouse masterdata is empty
        [Fact]
        public async Task WhenHaveParts_ButWarehouseMasterInSectionDataIsEmpty_ShouldReturnWarehouseAreCorrect()
        {
            using (var context = new ApplicationDbContext(
                    new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
             ))
            {
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var masterClient = new Mock<IMasterClient>();
                masterClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(default(PartModel)));
                masterClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(new WarehouseLocationModel
                {
                    Data = new List<WarehouseLocationModel.DataModel>()
                }));

                var service = CreatePartService(unitOfWork, masterClient.Object);

                var result = await service.GetPartsThatHaveAvailableQtyInStock(new Sieve.Models.SieveModel(), "w1");

                Assert.Equal("w1", result.Result.WarehouseCode);
                Assert.Null(result.Result.WarehouseDescription);
            }
        }

        [Fact]
        public async Task WhenHaveParts_ButWarehouseMasterIsEmpty_ShouldReturnWarehouseAreCorrect()
        {
            using (var context = new ApplicationDbContext(
                    new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
             ))
            {
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var masterClient = new Mock<IMasterClient>();
                masterClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(default(PartModel)));
                masterClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(new WarehouseLocationModel()));

                var service = CreatePartService(unitOfWork, masterClient.Object);

                var result = await service.GetPartsThatHaveAvailableQtyInStock(new Sieve.Models.SieveModel(), "w1");

                Assert.Equal("w1", result.Result.WarehouseCode);
                Assert.Null(result.Result.WarehouseDescription);
            }
        }

        // warehouse masterdata is null
        [Fact]
        public async Task WhenHaveParts_ButWarehouseMasterIsNull_ShouldReturnWarehouseAreCorrect()
        {
            using (var context = new ApplicationDbContext(
                    new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
             ))
            {
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var masterClient = new Mock<IMasterClient>();
                masterClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(default(PartModel)));
                masterClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(default(WarehouseLocationModel)));

                var service = CreatePartService(unitOfWork, masterClient.Object);

                var result = await service.GetPartsThatHaveAvailableQtyInStock(new Sieve.Models.SieveModel(), "w1");

                Assert.Equal("w1", result.Result.WarehouseCode);
                Assert.Null(result.Result.WarehouseDescription);
            }
        }
        #endregion

        #region Check available for GetPartsThatHaveAvailableQtyInStock by warehouse location
        [Fact]
        public async Task WhenHavePartAndHasAvailable_ShouldReturnThatPart()
        {
            using (var context = new ApplicationDbContext(
                    new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
             ))
            {
                MockData(context);
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var masterClient = new Mock<IMasterClient>();
                masterClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(default(PartModel)));
                masterClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(default(WarehouseLocationModel)));

                var service = CreatePartService(unitOfWork, masterClient.Object);

                var result = await service.GetPartsThatHaveAvailableQtyInStock(new Sieve.Models.SieveModel(), "w2");

                Assert.Equal("w2", result.Result.WarehouseCode);
                Assert.Single(result.Result.Rows);
                Assert.Equal(1, result.Result.TotalRows);
                var part1 = result.Result.Rows.First(x => x.PartNo == "p1");
                Assert.Equal(10, part1.Available);
            }
        }

        [Fact]
        public async Task WhenHavePartsAndDontHasAvailable_ShouldNotReturnThatParts()
        {
            using (var context = new ApplicationDbContext(
                    new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
             ))
            {
                MockData(context);
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var masterClient = new Mock<IMasterClient>();
                masterClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(default(PartModel)));
                masterClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(default(WarehouseLocationModel)));

                var service = CreatePartService(unitOfWork, masterClient.Object);

                var result = await service.GetPartsThatHaveAvailableQtyInStock(new Sieve.Models.SieveModel(), "w1");

                Assert.Equal("w1", result.Result.WarehouseCode);
                Assert.Equal(0, result.Result.TotalRows);
                Assert.Empty(result.Result.Rows);
            }
        }

        [Fact]
        public async Task WhenHavePartsAndSomePartHasNotAvailable_ShouldReturnThatPartHasAvailable()
        {
            using (var context = new ApplicationDbContext(
                    new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
             ))
            {
                MockData(context);
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var masterClient = new Mock<IMasterClient>();
                masterClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(default(PartModel)));
                masterClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(default(WarehouseLocationModel)));

                var service = CreatePartService(unitOfWork, masterClient.Object);

                var result = await service.GetPartsThatHaveAvailableQtyInStock(new Sieve.Models.SieveModel(), "w3");

                Assert.Equal("w3", result.Result.WarehouseCode);
                Assert.Equal(1, result.Result.TotalRows);
                Assert.Single(result.Result.Rows);
                Assert.Equal(5, result.Result.Rows.First(x => x.PartNo == "p2").Available);
            }
        }

        [Fact]
        public async Task WhenHavePartsAndHaveAvailable_ShouldReturnAllPartsHaveAvailable()
        {
            using (var context = new ApplicationDbContext(
                    new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
             ))
            {
                MockData(context);
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var masterClient = new Mock<IMasterClient>();
                masterClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(default(PartModel)));
                masterClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(default(WarehouseLocationModel)));

                var service = CreatePartService(unitOfWork, masterClient.Object);

                var result = await service.GetPartsThatHaveAvailableQtyInStock(new Sieve.Models.SieveModel(), "w4");

                Assert.Equal("w4", result.Result.WarehouseCode);
                Assert.Equal(2, result.Result.TotalRows);
                Assert.Equal(2, result.Result.Rows.Count());
                var part = result.Result.Rows.First(x => x.PartNo == "p1");
                Assert.Equal(2, part.Available);
                part = result.Result.Rows.First(x => x.PartNo == "p2");
                Assert.Equal(5, part.Available);
            }
        }

        [Fact]
        public async Task WhenDontHaveParts_ShouldReturnEmpty()
        {
            using (var context = new ApplicationDbContext(
                    new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
             ))
            {
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var masterClient = new Mock<IMasterClient>();
                masterClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(default(PartModel)));
                masterClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(default(WarehouseLocationModel)));

                var service = CreatePartService(unitOfWork, masterClient.Object);

                var result = await service.GetPartsThatHaveAvailableQtyInStock(new Sieve.Models.SieveModel(), "w4");

                Assert.Empty(result.Result.Rows);
                Assert.Equal(0, result.Result.TotalRows);
            }
        }

        #endregion

        #region Check available for GetPartsThatHaveAvailableQtyInStock all
        // Get parts all
        [Fact]
        public async Task WhenCall_ShouldReturnAllPartsHaveAvailable()
        {
            using (var context = new ApplicationDbContext(
                    new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
             ))
            {
                MockData(context);
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var masterClient = new Mock<IMasterClient>();
                masterClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(default(PartModel)));
                masterClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(default(WarehouseLocationModel)));

                var service = CreatePartService(unitOfWork, masterClient.Object);

                var result = await service.GetPartsThatHaveAvailableQtyInStock(new Sieve.Models.SieveModel());

                Assert.Equal(2, result.Result.Rows.Count());
                Assert.Equal(2, result.Result.TotalRows);
                var part = result.Result.Rows.First(x => x.PartNo == "p1");
                Assert.Equal(12, part.Available);
                part = result.Result.Rows.First(x => x.PartNo == "p2");
                Assert.Equal(10, part.Available);
            }
        }

        // Dont have parts
        [Fact]
        public async Task WhenCall_ButDontHaveParts_ShouldReturnEmpty()
        {
            using (var context = new ApplicationDbContext(
                    new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
             ))
            {
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var masterClient = new Mock<IMasterClient>();
                masterClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(default(PartModel)));
                masterClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(default(WarehouseLocationModel)));

                var service = CreatePartService(unitOfWork, masterClient.Object);

                var result = await service.GetPartsThatHaveAvailableQtyInStock(new Sieve.Models.SieveModel());

                Assert.Empty(result.Result.Rows);
                Assert.Equal(0, result.Result.TotalRows);
            }
        }
        #endregion

        private void MockData(ApplicationDbContext context)
        {
            context.Inventories.AddRange(
                new Infrastructure.Entities.Inventory
                {
                    WarehouseLocationNo = "w1",
                    PartNo = "p1",
                    AvailableQty = 0
                },
                new Infrastructure.Entities.Inventory
                {
                    WarehouseLocationNo = "w2",
                    PartNo = "p1",
                    AvailableQty = 10
                },
                new Infrastructure.Entities.Inventory
                {
                    WarehouseLocationNo = "w3",
                    PartNo = "p1",
                    AvailableQty = 0
                },
                new Infrastructure.Entities.Inventory
                {
                    WarehouseLocationNo = "w3",
                    PartNo = "p2",
                    AvailableQty = 5
                },
                new Infrastructure.Entities.Inventory
                {
                    WarehouseLocationNo = "w4",
                    PartNo = "p1",
                    AvailableQty = 2
                },
                new Infrastructure.Entities.Inventory
                {
                    WarehouseLocationNo = "w4",
                    PartNo = "p2",
                    AvailableQty = 5
                }
            );
            context.SaveChanges();
        }
    }
}
