using InventoryManagement.Domain.Services;
using InventoryManagement.Infrastructure;
using InventoryManagement.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace InventoryManagement.Tests.Services.InventoryServiceTest
{
    public class ValidateQuantityTest
    {
        private static InventoryService CreateInventoryService(UnitOfWork unitOfWork)
            => new InventoryService(unitOfWork, null, null, null, null, null);
        
        // Available enough all
        [Fact]
        public async Task WhenBothItemsHaveAvailableQtyAreEnough_ShouldReturnResultAreCorrect()
        {
            using (var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
            ))
            {
                MockData(context);
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var payload = new Domain.DTO.Requests.InventoryRequest
                {
                    WarehouseLocationNo = "w1",
                    RequestOrderNo = "",
                    Parts = new List<Domain.DTO.Requests.Part> {
                    new Domain.DTO.Requests.Part
                    {
                        No = "p1",
                        Qty = 4,
                        Uom = "pcs"
                    },
                    new Domain.DTO.Requests.Part
                    {
                        No = "p2",
                        Qty = 10,
                        Uom = "pcs"
                    }
                }

                };
                var service = CreateInventoryService(unitOfWork);

                var result = await service.ValidateQuantity(payload);

                var item1 = result.First(x => x.PartNo == "p1");
                Assert.Equal("p1", item1.PartNo);
                Assert.Equal(4, item1.Qty);
                Assert.Equal("pcs", item1.Uom);
                Assert.Equal(5, item1.AvailableQuantity);
                Assert.True(item1.IsEnough);

                var item2 = result.First(x => x.PartNo == "p2");
                Assert.Equal("p2", item2.PartNo);
                Assert.Equal(10, item2.Qty);
                Assert.Equal("pcs", item2.Uom);
                Assert.Equal(10, item2.AvailableQuantity);
                Assert.True(item2.IsEnough);
            }
        }

        // first available enough but second item not enough
        [Fact]
        public async Task WhenFirstItemHasAvailableQtyIsEnough_ButSecondItemHasAvailableQtyItemIsNotEnough_ShouldReturnIsEnoughItem1IsTrueAndItem2IsFalse()
        {
            using (var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
            ))
            {
                MockData(context);
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var payload = new Domain.DTO.Requests.InventoryRequest
                {
                    WarehouseLocationNo = "w1",
                    RequestOrderNo = "",
                    Parts = new List<Domain.DTO.Requests.Part> {
                    new Domain.DTO.Requests.Part
                    {
                        No = "p1",
                        Qty = 5,
                        Uom = "pcs"
                    },
                    new Domain.DTO.Requests.Part
                    {
                        No = "p2",
                        Qty = 11,
                        Uom = "pcs"
                    }
                }

                };
                var service = CreateInventoryService(unitOfWork);

                var result = await service.ValidateQuantity(payload);

                var item1 = result.First(x => x.PartNo == "p1");
                Assert.Equal("p1", item1.PartNo);
                Assert.Equal(5, item1.Qty);
                Assert.Equal("pcs", item1.Uom);
                Assert.Equal(5, item1.AvailableQuantity);
                Assert.True(item1.IsEnough);

                var item2 = result.First(x => x.PartNo == "p2");
                Assert.Equal("p2", item2.PartNo);
                Assert.Equal(11, item2.Qty);
                Assert.Equal("pcs", item2.Uom);
                Assert.Equal(10, item2.AvailableQuantity);
                Assert.False(item2.IsEnough);
            }
        }

        // second item enough but first not enough
        [Fact]
        public async Task WhenFirstItemHasAvailableQtyIsNotEnough_ButSecondItemHasAvailableQtyIsEnough_ShouldReturnIsEnoughInItem1IsFalseAndItem2IsTrue()
        {
            using (var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
            ))
            {
                MockData(context);
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var payload = new Domain.DTO.Requests.InventoryRequest
                {
                    WarehouseLocationNo = "w1",
                    RequestOrderNo = "",
                    Parts = new List<Domain.DTO.Requests.Part> {
                    new Domain.DTO.Requests.Part
                    {
                        No = "p1",
                        Qty = 6,
                        Uom = "pcs"
                    },
                    new Domain.DTO.Requests.Part
                    {
                        No = "p2",
                        Qty = 9,
                        Uom = "pcs"
                    }
                }

                };
                var service = CreateInventoryService(unitOfWork);

                var result = await service.ValidateQuantity(payload);

                var item1 = result.First(x => x.PartNo == "p1");
                Assert.Equal("p1", item1.PartNo);
                Assert.Equal(6, item1.Qty);
                Assert.Equal("pcs", item1.Uom);
                Assert.Equal(5, item1.AvailableQuantity);
                Assert.False(item1.IsEnough);

                var item2 = result.First(x => x.PartNo == "p2");
                Assert.Equal("p2", item2.PartNo);
                Assert.Equal(9, item2.Qty);
                Assert.Equal("pcs", item2.Uom);
                Assert.Equal(10, item2.AvailableQuantity);
                Assert.True(item2.IsEnough);
            }
        }

        // first and second aren't enough
        [Fact]
        public async Task WhenFirstAndSecondItemsHaveAvailableQtyAreNotEnough_ShouldReturnIsEnoughAreFalse()
        {
            using (var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
            ))
            {
                MockData(context);
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var payload = new Domain.DTO.Requests.InventoryRequest
                {
                    WarehouseLocationNo = "w1",
                    RequestOrderNo = "",
                    Parts = new List<Domain.DTO.Requests.Part> {
                    new Domain.DTO.Requests.Part
                    {
                        No = "p1",
                        Qty = 6,
                        Uom = "pcs"
                    },
                    new Domain.DTO.Requests.Part
                    {
                        No = "p2",
                        Qty = 12,
                        Uom = "pcs"
                    }
                }

                };
                var service = CreateInventoryService(unitOfWork);

                var result = await service.ValidateQuantity(payload);

                var item1 = result.First(x => x.PartNo == "p1");
                Assert.Equal("p1", item1.PartNo);
                Assert.Equal(6, item1.Qty);
                Assert.Equal("pcs", item1.Uom);
                Assert.Equal(5, item1.AvailableQuantity);
                Assert.False(item1.IsEnough);

                var item2 = result.First(x => x.PartNo == "p2");
                Assert.Equal("p2", item2.PartNo);
                Assert.Equal(12, item2.Qty);
                Assert.Equal("pcs", item2.Uom);
                Assert.Equal(10, item2.AvailableQuantity);
                Assert.False(item2.IsEnough);
            }
        }

        // partNo not found
        [Fact]
        public async Task WhenPartNoNotFoundInInventorySystem_ShouldReturnEmptyAvailableQty()
        {
            using (var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
            ))
            {
                MockData(context);
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var payload = new Domain.DTO.Requests.InventoryRequest
                {
                    WarehouseLocationNo = "w1",
                    RequestOrderNo = "",
                    Parts = new List<Domain.DTO.Requests.Part> {
                    new Domain.DTO.Requests.Part
                    {
                        No = "abc123",
                        Qty = 6,
                        Uom = "pcs"
                    }
                }
                };
                var service = CreateInventoryService(unitOfWork);

                var result = await service.ValidateQuantity(payload);

                var item1 = result.First(x => x.PartNo == "abc123");
                Assert.Equal("abc123", item1.PartNo);
                Assert.Equal(6, item1.Qty);
                Assert.Equal("pcs", item1.Uom);
                Assert.Equal(0, item1.AvailableQuantity);
                Assert.False(item1.IsEnough);
            }
        }

        // warehouse not found
        [Fact]
        public async Task WhenWharehouseNotFoundInInventorySystem_ShouldReturnEmptyAvailableQty()
        {
            using (var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
            ))
            {
                MockData(context);
                var unitOfWork = new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor()));
                var payload = new Domain.DTO.Requests.InventoryRequest
                {
                    WarehouseLocationNo = "ware123",
                    RequestOrderNo = "",
                    Parts = new List<Domain.DTO.Requests.Part> {
                    new Domain.DTO.Requests.Part
                    {
                        No = "p1",
                        Qty = 6,
                        Uom = "pcs"
                    }
                }
                };
                var service = CreateInventoryService(unitOfWork);

                var result = await service.ValidateQuantity(payload);

                var item1 = result.First(x => x.PartNo == "p1");
                Assert.Equal("p1", item1.PartNo);
                Assert.Equal(6, item1.Qty);
                Assert.Equal("pcs", item1.Uom);
                Assert.Equal(0, item1.AvailableQuantity);
                Assert.False(item1.IsEnough);
            }
        }

        private void MockData(ApplicationDbContext context)
        {
            context.Inventories.Add(new Inventory
            {
                PartNo = "p1",
                WarehouseLocationNo = "w1",
                AvailableQty = 5
            });
            context.Inventories.Add(new Inventory
            {
                PartNo = "p2",
                WarehouseLocationNo = "w1",
                AvailableQty = 10
            });
            context.SaveChanges();
        }
    }
}
