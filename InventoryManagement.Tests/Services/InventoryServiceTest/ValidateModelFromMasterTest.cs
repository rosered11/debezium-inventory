using InventoryManagement.Domain.Client;
using InventoryManagement.Domain.Client.Models.MasterClient;
using InventoryManagement.Domain.DTO.Requests;
using InventoryManagement.Domain.Services;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace InventoryManagement.Tests.Services.InventoryServiceTest
{
    public class ValidateModelFromMasterTest
    {
        private static InventoryService CreateInventoryService(IMasterClient masterClient)
            => new InventoryService(null, null, null, masterClient, null, null);

        // PartNo and warehouse match
        [Fact]
        public async Task WhenPartNoAndWarehouseLocationNoAreMatchedInMaster_ShouldReturnTrue()
        {
            var request = new Inventory
            {
                PartNo = "p1",
                Quantity = 1,
                TransactionType = "",
                WarehouseLocationNo = "w1",
                Uom = "uom"
            };
            Mock<IMasterClient> mockClient = new Mock<IMasterClient>();
            mockClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(new PartModel { Data = new List<PartModel.DataModel> { 
                new PartModel.DataModel { No = "p1", UomId = new PartModel.UomModel { Name = "uom" } } 
            }}));
            mockClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(new WarehouseLocationModel
            {
                Data = new List<WarehouseLocationModel.DataModel> {
                new WarehouseLocationModel.DataModel { Code = "w1"  }
            }}));
            var service = CreateInventoryService(mockClient.Object);

            var result = await service.ValidateModelFromMaster(request);

            Assert.True(result);
        }

        // PartNo match only
        [Fact]
        public async Task WhenPartNoIsMatched_ButWarehouseLocationNoNotMatch_ShouldReturnFalse()
        {
            var request = new Inventory
            {
                PartNo = "p1",
                Quantity = 1,
                TransactionType = "",
                WarehouseLocationNo = "w2",
                Uom = "uom"
            };
            Mock<IMasterClient> mockClient = new Mock<IMasterClient>();
            mockClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(new PartModel
            {
                Data = new List<PartModel.DataModel> {
                new PartModel.DataModel { No = "p1", UomId = new PartModel.UomModel { Name = "uom" } }
            }
            }));
            mockClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(new WarehouseLocationModel
            {
                Data = new List<WarehouseLocationModel.DataModel> {
                new WarehouseLocationModel.DataModel { Code = "w1"  }
            }
            }));
            var service = CreateInventoryService(mockClient.Object);

            var result = await service.ValidateModelFromMaster(request);

            Assert.False(result);
        }

        // warehouse match only
        [Fact]
        public async Task WhenWarehouseLocationNoIsMatched_ButPartNoIsNotMatch_ShouldReturnFalse()
        {
            var request = new Inventory
            {
                PartNo = "p1",
                Quantity = 1,
                TransactionType = "",
                WarehouseLocationNo = "w1"
            };
            Mock<IMasterClient> mockClient = new Mock<IMasterClient>();
            mockClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(new PartModel
            {
                Data = new List<PartModel.DataModel> {
                new PartModel.DataModel { No = "p2" }
            }
            }));
            mockClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(new WarehouseLocationModel
            {
                Data = new List<WarehouseLocationModel.DataModel> {
                new WarehouseLocationModel.DataModel { Code = "w1"  }
            }
            }));
            var service = CreateInventoryService(mockClient.Object);

            var result = await service.ValidateModelFromMaster(request);

            Assert.False(result);
        }

        // without match
        [Fact]
        public async Task WhenWarehouseLocationAndPartNoAreNotMatch_ShouldReturlFalse()
        {
            var request = new Inventory
            {
                PartNo = "p2",
                Quantity = 1,
                TransactionType = "",
                WarehouseLocationNo = "w2"
            };
            Mock<IMasterClient> mockClient = new Mock<IMasterClient>();
            mockClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(new PartModel
            {
                Data = new List<PartModel.DataModel> {
                new PartModel.DataModel { No = "p1" }
            }
            }));
            mockClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(new WarehouseLocationModel
            {
                Data = new List<WarehouseLocationModel.DataModel> {
                new WarehouseLocationModel.DataModel { Code = "w1"  }
            }
            }));
            var service = CreateInventoryService(mockClient.Object);

            var result = await service.ValidateModelFromMaster(request);

            Assert.False(result);
        }

        [Fact]
        public async Task WhenPartNoAndWarehouseLocationNoAreMatchedInMaster_ButUomNotMatch_ShouldReturnFalse()
        {
            var request = new Inventory
            {
                PartNo = "p1",
                Quantity = 1,
                TransactionType = "",
                WarehouseLocationNo = "w1",
                Uom = "uom"
            };
            Mock<IMasterClient> mockClient = new Mock<IMasterClient>();
            mockClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(new PartModel
            {
                Data = new List<PartModel.DataModel> {
                new PartModel.DataModel { No = "p1", UomId = new PartModel.UomModel { Name = "uom2" } }
            }
            }));
            mockClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(new WarehouseLocationModel
            {
                Data = new List<WarehouseLocationModel.DataModel> {
                new WarehouseLocationModel.DataModel { Code = "w1"  }
            }
            }));
            var service = CreateInventoryService(mockClient.Object);

            var result = await service.ValidateModelFromMaster(request);

            Assert.False(result);
        }

        [Fact]
        public async Task WhenPartNoAndWarehouseLocationNoAreMatchedInMaster_ButUomIsNull_ShouldReturnFalse()
        {
            var request = new Inventory
            {
                PartNo = "p1",
                Quantity = 1,
                TransactionType = "",
                WarehouseLocationNo = "w1",
                Uom = "uom"
            };
            Mock<IMasterClient> mockClient = new Mock<IMasterClient>();
            mockClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(new PartModel
            {
                Data = new List<PartModel.DataModel> {
                new PartModel.DataModel { No = "p1", UomId = null }
            }
            }));
            mockClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(new WarehouseLocationModel
            {
                Data = new List<WarehouseLocationModel.DataModel> {
                new WarehouseLocationModel.DataModel { Code = "w1"  }
            }
            }));
            var service = CreateInventoryService(mockClient.Object);

            var result = await service.ValidateModelFromMaster(request);

            Assert.False(result);
        }

        [Fact]
        public async Task WhenPartNoAndWarehouseLocationNoAreMatchedInMaster_ButUomIsEmpty_ShouldReturnFalse()
        {
            var request = new Inventory
            {
                PartNo = "p1",
                Quantity = 1,
                TransactionType = "",
                WarehouseLocationNo = "w1",
                Uom = "uom"
            };
            Mock<IMasterClient> mockClient = new Mock<IMasterClient>();
            mockClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(new PartModel
            {
                Data = new List<PartModel.DataModel> {
                new PartModel.DataModel { No = "p1", UomId = new PartModel.UomModel() }
            }
            }));
            mockClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(new WarehouseLocationModel
            {
                Data = new List<WarehouseLocationModel.DataModel> {
                new WarehouseLocationModel.DataModel { Code = "w1"  }
            }
            }));
            var service = CreateInventoryService(mockClient.Object);

            var result = await service.ValidateModelFromMaster(request);

            Assert.False(result);
        }
    }
}
