using Boxed.Mapping;
using InventoryManagement.Domain.Client;
using InventoryManagement.Domain.Client.Models.MasterClient;
using InventoryManagement.Domain.Mapper;
using InventoryManagement.Domain.Services;
using InventoryManagement.Infrastructure;
using InventoryManagement.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace InventoryManagement.Tests.Services.InventoryServiceTest
{
    public class GetStockInventoryTest
    {
        private static InventoryService CreateInventoryService(UnitOfWork unit, IMapper<Inventory, Domain.DTO.Responses.Row> mapper, IMasterClient masterClient) 
            => new InventoryService(unit, mapper, null, masterClient, null, null);
        [Fact]
        public async Task WhenHasWarehouseNoAndPartNo_AndConditionIsMatch_ShouldReturnResultsAreCorrect()
        {
            using (var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
            ))
            {
                MockData(context);
                var mapper = new InventoryToInventoryRowResponseMapper();
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var masterClient = new Mock<IMasterClient>();
                masterClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(new PartModel { Data = new List<PartModel.DataModel> { 
                    new PartModel.DataModel
                    {
                        No = "partno1",
                        Name = "partname1"
                    }
                } }));
                var service = CreateInventoryService(unitOfWork, mapper, masterClient.Object);

                var result = await service.GetStockInventoryAsync(new Sieve.Models.SieveModel(), warehouseLocation: "warehouse1", partNo: "partno1");

                Assert.NotNull(result);
                Assert.Single(result.Result.Rows);
                Assert.Equal(1, result.Result.TotalRows);
                Assert.Equal("partno1", result.Result.Rows.First().PartNo);
                Assert.Equal("warehouse1", result.Result.WarehouseLocationNo);
                Assert.Equal("partname1", result.Result.Rows.First().PartName);
                Assert.Equal(1, result.Result.Rows.First().AvailableQuantity);
                Assert.Equal(2, result.Result.Rows.First().BalanceQuantity);
                Assert.Equal(3, result.Result.Rows.First().ReceivingQuantity);
                Assert.Equal(4, result.Result.Rows.First().PoQuantity);
                Assert.Equal(5, result.Result.Rows.First().RequestQuantity);
                Assert.Equal("uom", result.Result.Rows.First().Uom);
            }
        }

        [Fact]
        public async Task WhenHasWarehouseNoOnly_AndConditionIsMatch_ShouldReturnResultsAreCorrect()
        {
            using (var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
            ))
            {
                MockData(context);
                var mapper = new InventoryToInventoryRowResponseMapper();
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var masterClient = new Mock<IMasterClient>();
                masterClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(new PartModel
                {
                    Data = new List<PartModel.DataModel> {
                    new PartModel.DataModel
                    {
                        No = "partno1",
                        Name = "partname1"
                    },
                    new PartModel.DataModel
                    {
                        No = "partno2",
                        Name = "partname2"
                    }
                }
                }));
                var service = CreateInventoryService(unitOfWork, mapper, masterClient.Object);

                var result = await service.GetStockInventoryAsync(new Sieve.Models.SieveModel(), warehouseLocation: "warehouse1", partNo: null);

                Assert.NotNull(result);
                Assert.Equal(2, result.Result.Rows.Count());
                Assert.Equal(2, result.Result.TotalRows);
                Assert.NotNull(result.Result.Rows.FirstOrDefault(x => x.PartNo == "partno1"));
                Assert.NotNull(result.Result.Rows.FirstOrDefault(x => x.PartNo == "partno2"));
                Assert.Equal("warehouse1", result.Result.WarehouseLocationNo);
                var part = result.Result.Rows.FirstOrDefault(x => x.PartNo == "partno1");
                Assert.Equal("partname1", part.PartName);
                Assert.Equal(1, part.AvailableQuantity);
                Assert.Equal(2, part.BalanceQuantity);
                Assert.Equal(3, part.ReceivingQuantity);
                Assert.Equal(4, part.PoQuantity);
                Assert.Equal(5, part.RequestQuantity);
                Assert.Equal("uom", part.Uom);
                part = result.Result.Rows.FirstOrDefault(x => x.PartNo == "partno2");
                Assert.Equal("partname2", part.PartName);
                Assert.Equal(1, part.AvailableQuantity);
                Assert.Equal(2, part.BalanceQuantity);
                Assert.Equal(3, part.ReceivingQuantity);
                Assert.Equal(4, part.PoQuantity);
                Assert.Equal(5, part.RequestQuantity);
                Assert.Equal("uom", part.Uom);
            }
        }

        [Fact]
        public async Task WhenHasPartNoOnly_AndConditionIsMatch_ShouldReturnResultsAreCorrect()
        {
            using (var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
            ))
            {
                MockData(context);
                var mapper = new InventoryToInventoryRowResponseMapper();
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var masterClient = new Mock<IMasterClient>();
                masterClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(new PartModel
                {
                    Data = new List<PartModel.DataModel> {
                    new PartModel.DataModel
                    {
                        No = "partno1",
                        Name = "partname1"
                    },
                    new PartModel.DataModel
                    {
                        No = "partno2",
                        Name = "partname2"
                    }
                }
                }));
                var service = CreateInventoryService(unitOfWork, mapper, masterClient.Object);

                var result = await service.GetStockInventoryAsync(new Sieve.Models.SieveModel(), warehouseLocation: null, partNo: "partno2");

                Assert.NotNull(result.Result.Rows.FirstOrDefault(x => x.PartNo == "partno2"));
                Assert.Single(result.Result.Rows);
                Assert.Equal(1, result.Result.TotalRows);
                Assert.Equal("ALL", result.Result.WarehouseLocationNo);
                Assert.Single(result.Result.Rows.Where(x => x.PartName == "partname2"));
                var part = result.Result.Rows.FirstOrDefault(x => x.PartNo == "partno2");
                Assert.Equal(2, part.AvailableQuantity);
                Assert.Equal(4, part.BalanceQuantity);
                Assert.Equal(6, part.ReceivingQuantity);
                Assert.Equal(8, part.PoQuantity);
                Assert.Equal(10, part.RequestQuantity);
                Assert.Equal("uom", part.Uom);
            }
        }

        [Fact]
        public async Task WhenWithoutValues_ShouldReturnResultsAreCorrect()
        {
            using (var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
            ))
            {
                MockData(context);
                var mapper = new InventoryToInventoryRowResponseMapper();
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var masterClient = new Mock<IMasterClient>();
                masterClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(new PartModel
                {
                    Data = new List<PartModel.DataModel> {
                    new PartModel.DataModel
                    {
                        No = "partno1",
                        Name = "partname1"
                    },
                    new PartModel.DataModel
                    {
                        No = "partno2",
                        Name = "partname2"
                    }
                }
                }));
                var service = CreateInventoryService(unitOfWork, mapper, masterClient.Object);

                var result = await service.GetStockInventoryAsync(new Sieve.Models.SieveModel(), warehouseLocation: null, partNo: null);

                Assert.Equal(2, result.Result.Rows.Count());
                Assert.Equal(2, result.Result.TotalRows);
                Assert.Single(result.Result.Rows.Where(x => x.PartNo == "partno1"));
                Assert.Single(result.Result.Rows.Where(x => x.PartNo == "partno2"));
                Assert.Single(result.Result.Rows.Where(x => x.PartName == "partname1"));
                Assert.Single(result.Result.Rows.Where(x => x.PartName == "partname2"));
                Assert.Equal("ALL", result.Result.WarehouseLocationNo);
                var part = result.Result.Rows.First(x => x.PartNo == "partno1");
                Assert.Equal(3, part.AvailableQuantity);
                Assert.Equal(6, part.BalanceQuantity);
                Assert.Equal(9, part.ReceivingQuantity);
                Assert.Equal(12, part.PoQuantity);
                Assert.Equal(15, part.RequestQuantity);
                Assert.Equal("uom", part.Uom);
                part = result.Result.Rows.First(x => x.PartNo == "partno2");
                Assert.Equal(2, part.AvailableQuantity);
                Assert.Equal(4, part.BalanceQuantity);
                Assert.Equal(6, part.ReceivingQuantity);
                Assert.Equal(8, part.PoQuantity);
                Assert.Equal(10, part.RequestQuantity);
                Assert.Equal("uom", part.Uom);
            }
        }

        [Fact]
        public async Task WhenHasPartNoAndWarehouseNo_ButConditionNotMatch_ShouldReturnEmptyRow()
        {
            using (var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
            ))
            {
                MockData(context);
                var mapper = new InventoryToInventoryRowResponseMapper();
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var masterClient = new Mock<IMasterClient>();
                masterClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(new PartModel
                {
                    Data = new List<PartModel.DataModel> {
                    new PartModel.DataModel
                    {
                        No = "partno1",
                        Name = "partname1"
                    },
                    new PartModel.DataModel
                    {
                        No = "partno2",
                        Name = "partname2"
                    }
                }
                }));
                var service = CreateInventoryService(unitOfWork, mapper, masterClient.Object);

                var result = await service.GetStockInventoryAsync(new Sieve.Models.SieveModel(), warehouseLocation: "warehouse1", partNo: "partno3");

                Assert.Empty(result.Result.Rows);
                Assert.Equal(0, result.Result.TotalRows);
                Assert.Equal("warehouse1", result.Result.WarehouseLocationNo);
            }
        }

        [Fact]
        public async Task WhenHasPartNoOnly_ButConditionNotMatch_ShouldReturnEmptyRow()
        {
            using (var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
            ))
            {
                MockData(context);
                var mapper = new InventoryToInventoryRowResponseMapper();
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var masterClient = new Mock<IMasterClient>();
                masterClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(new PartModel
                {
                    Data = new List<PartModel.DataModel> {
                    new PartModel.DataModel
                    {
                        No = "partno1",
                        Name = "partname1"
                    },
                    new PartModel.DataModel
                    {
                        No = "partno2",
                        Name = "partname2"
                    }
                }
                }));
                var service = CreateInventoryService(unitOfWork, mapper, masterClient.Object);

                var result = await service.GetStockInventoryAsync(new Sieve.Models.SieveModel(), warehouseLocation: null, partNo: "partno3");

                Assert.Empty(result.Result.Rows);
                Assert.Equal(0, result.Result.TotalRows);
                Assert.Equal("ALL", result.Result.WarehouseLocationNo);
            }
        }

        [Fact]
        public async Task WhenHasWarehouseNoAndPartNo_AndConditionIsMatch_AndDontHavePartNameInMasterData_ShouldReturnResultsAreCorrect()
        {
            using (var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
            ))
            {
                MockData(context);
                var mapper = new InventoryToInventoryRowResponseMapper();
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var masterClient = new Mock<IMasterClient>();
                masterClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(default(PartModel)));
                var service = CreateInventoryService(unitOfWork, mapper, masterClient.Object);

                var result = await service.GetStockInventoryAsync(new Sieve.Models.SieveModel(), warehouseLocation: "warehouse1", partNo: "partno1");

                Assert.NotNull(result);
                Assert.Single(result.Result.Rows);
                Assert.Equal(1, result.Result.TotalRows);
                Assert.Equal("partno1", result.Result.Rows.First().PartNo);
                Assert.Equal("warehouse1", result.Result.WarehouseLocationNo);
                Assert.Null(result.Result.Rows.First().PartName);
                var part = result.Result.Rows.First();
                Assert.Equal(1, part.AvailableQuantity);
                Assert.Equal(2, part.BalanceQuantity);
                Assert.Equal(3, part.ReceivingQuantity);
                Assert.Equal(4, part.PoQuantity);
                Assert.Equal(5, part.RequestQuantity);
                Assert.Equal("uom", part.Uom);
            }
        }

        private void MockData(ApplicationDbContext context)
        {
            context.Inventories.Add(new Inventory
            {
                PartNo = "partno1",
                WarehouseLocationNo = "warehouse1",
                AvailableQty = 1,
                BalanceQty = 2,
                ReceivingQty = 3,
                POQty = 4,
                Uom = "uom",
                RequestQty = 5
            });
            context.Inventories.Add(new Inventory
            {
                PartNo = "partno2",
                WarehouseLocationNo = "warehouse1",
                AvailableQty = 1,
                BalanceQty = 2,
                ReceivingQty = 3,
                POQty = 4,
                Uom = "uom",
                RequestQty = 5
            });
            context.Inventories.Add(new Inventory
            {
                PartNo = "partno1",
                WarehouseLocationNo = "warehouse2",
                AvailableQty = 1,
                BalanceQty = 2,
                ReceivingQty = 3,
                POQty = 4,
                Uom = "uom",
                RequestQty = 5
            });
            context.Inventories.Add(new Inventory
            {
                PartNo = "partno2",
                WarehouseLocationNo = "warehouse2",
                AvailableQty = 1,
                BalanceQty = 2,
                ReceivingQty = 3,
                POQty = 4,
                Uom = "uom",
                RequestQty = 5
            });
            context.Inventories.Add(new Inventory
            {
                PartNo = "partno1",
                WarehouseLocationNo = "warehouse3",
                AvailableQty = 1,
                BalanceQty = 2,
                ReceivingQty = 3,
                POQty = 4,
                Uom = "uom",
                RequestQty = 5
            });
            context.SaveChanges();
        }
    }
}
