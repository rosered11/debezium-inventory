using Boxed.Mapping;
using InventoryManagement.Domain.Client;
using InventoryManagement.Domain.Client.Models.MasterClient;
using InventoryManagement.Domain.Mapper;
using InventoryManagement.Domain.Services;
using InventoryManagement.Domain.Utilities;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace InventoryManagement.Tests.Services.InventoryServiceTest
{
    public class ValidateModelListFromMaster
    {
        private static InventoryService CreateInventoryService(IMasterClient masterClient, IMapper<Domain.DTO.Requests.Part, Domain.DTO.Requests.Inventory> mapper)
            => new InventoryService(null, null, null, masterClient, mapper, null);

        [Fact]
        public async Task WhenBothPartNoInListAndWarehouseLocationNoHaveOnMaster_ShouldReturnTrue()
        {
            var mapper = new InvetoryRequestPartToInventoryRequestMapper();
            Mock<IMasterClient> mockClient = new Mock<IMasterClient>();
            mockClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(new PartModel
            {
                Data = new List<PartModel.DataModel> {
                new PartModel.DataModel { No = "p1", UomId = new PartModel.UomModel { Name = "uom1" } },
                new PartModel.DataModel { No = "p2", UomId = new PartModel.UomModel { Name = "uom2" } }
            }
            }));
            mockClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(new WarehouseLocationModel
            {
                Data = new List<WarehouseLocationModel.DataModel> {
                new WarehouseLocationModel.DataModel { Code = "w1"  }
            }
            }));
            var request = new Domain.DTO.Requests.InventoryRequest
            {
                WarehouseLocationNo = "w1",
                Parts = new List<Domain.DTO.Requests.Part>{
                    new Domain.DTO.Requests.Part
                    {
                        No = "p1", Uom = "uom1"
                    },
                    new Domain.DTO.Requests.Part
                    {
                        No = "p2", Uom = "uom2"
                    },
                }
            };
            var service = CreateInventoryService(mockClient.Object, mapper);

            var result = await service.ValidateModelListFromMaster(request, InputTransactionType.RQ_SUBMITTED);

            Assert.True(result);
        }

        [Fact]
        public async Task WhenSomePartNoInListNotFoundOnMaster_ShouldReturnFalse()
        {
            var mapper = new InvetoryRequestPartToInventoryRequestMapper();
            Mock<IMasterClient> mockClient = new Mock<IMasterClient>();
            mockClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(new PartModel
            {
                Data = new List<PartModel.DataModel> {
                new PartModel.DataModel { No = "p1", UomId = new PartModel.UomModel { Name = "uom1" } },
                new PartModel.DataModel { No = "p2", UomId = new PartModel.UomModel { Name = "uom2" } }
            }
            }));
            mockClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(new WarehouseLocationModel
            {
                Data = new List<WarehouseLocationModel.DataModel> {
                new WarehouseLocationModel.DataModel { Code = "w1"  }
            }
            }));
            var request = new Domain.DTO.Requests.InventoryRequest
            {
                WarehouseLocationNo = "w1",
                Parts = new List<Domain.DTO.Requests.Part>{
                    new Domain.DTO.Requests.Part
                    {
                        No = "p3", Uom = "uom3"
                    },
                    new Domain.DTO.Requests.Part
                    {
                        No = "p2", Uom = "uom2"
                    },
                }
            };
            var service = CreateInventoryService(mockClient.Object, mapper);

            var result = await service.ValidateModelListFromMaster(request, InputTransactionType.RQ_SUBMITTED);

            Assert.False(result);
        }

        [Fact]
        public async Task WhenWarehouseLocationNotFoundOnMaster_ShouldReturnFalse()
        {
            var mapper = new InvetoryRequestPartToInventoryRequestMapper();
            Mock<IMasterClient> mockClient = new Mock<IMasterClient>();
            mockClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(new PartModel
            {
                Data = new List<PartModel.DataModel> {
                new PartModel.DataModel { No = "p1", UomId = new PartModel.UomModel { Name = "uom1" } },
                new PartModel.DataModel { No = "p2", UomId = new PartModel.UomModel { Name = "uom2" } }
            }
            }));
            mockClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(new WarehouseLocationModel
            {
                Data = new List<WarehouseLocationModel.DataModel> {
                new WarehouseLocationModel.DataModel { Code = "w1"  }
            }
            }));
            var request = new Domain.DTO.Requests.InventoryRequest
            {
                WarehouseLocationNo = "w2",
                Parts = new List<Domain.DTO.Requests.Part>{
                    new Domain.DTO.Requests.Part
                    {
                        No = "p1", Uom = "uom1"
                    },
                    new Domain.DTO.Requests.Part
                    {
                        No = "p2", Uom = "uom2"
                    },
                }
            };
            var service = CreateInventoryService(mockClient.Object, mapper);

            var result = await service.ValidateModelListFromMaster(request, InputTransactionType.RQ_SUBMITTED);

            Assert.False(result);
        }

        [Fact]
        public async Task WhenBothAllPartNoAndWarehouseLocationNotFoundOnMaster_ShouldReturnFalse()
        {
            var mapper = new InvetoryRequestPartToInventoryRequestMapper();
            Mock<IMasterClient> mockClient = new Mock<IMasterClient>();
            mockClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(new PartModel
            {
                Data = new List<PartModel.DataModel> {
                new PartModel.DataModel { No = "p1", UomId = new PartModel.UomModel { Name = "uom1" } },
                new PartModel.DataModel { No = "p2", UomId = new PartModel.UomModel { Name = "uom2" } }
            }
            }));
            mockClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(new WarehouseLocationModel
            {
                Data = new List<WarehouseLocationModel.DataModel> {
                new WarehouseLocationModel.DataModel { Code = "w1"  }
            }
            }));
            var request = new Domain.DTO.Requests.InventoryRequest
            {
                WarehouseLocationNo = "w2",
                Parts = new List<Domain.DTO.Requests.Part>{
                    new Domain.DTO.Requests.Part
                    {
                        No = "p3", Uom = "uom3"
                    },
                    new Domain.DTO.Requests.Part
                    {
                        No = "p4", Uom = "uom4"
                    },
                }
            };
            var service = CreateInventoryService(mockClient.Object, mapper);

            var result = await service.ValidateModelListFromMaster(request, InputTransactionType.RQ_SUBMITTED);

            Assert.False(result);
        }

        [Fact]
        public async Task WhenPartNoAndWarehouseLocationNoAreMatchedInMaster_ButSomeUomNotMatch_ShouldReturnFalse()
        {
            var mapper = new InvetoryRequestPartToInventoryRequestMapper();
            Mock<IMasterClient> mockClient = new Mock<IMasterClient>();
            mockClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(new PartModel
            {
                Data = new List<PartModel.DataModel> {
                new PartModel.DataModel { No = "p1", UomId = new PartModel.UomModel { Name = "uom1" } },
                new PartModel.DataModel { No = "p2", UomId = new PartModel.UomModel { Name = "uom2" } }
            }
            }));
            mockClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(new WarehouseLocationModel
            {
                Data = new List<WarehouseLocationModel.DataModel> {
                new WarehouseLocationModel.DataModel { Code = "w1"  }
            }
            }));
            var request = new Domain.DTO.Requests.InventoryRequest
            {
                WarehouseLocationNo = "w1",
                Parts = new List<Domain.DTO.Requests.Part>{
                    new Domain.DTO.Requests.Part
                    {
                        No = "p1", Uom = "uom3"
                    },
                    new Domain.DTO.Requests.Part
                    {
                        No = "p2", Uom = "uom2"
                    },
                }
            };
            var service = CreateInventoryService(mockClient.Object, mapper);

            var result = await service.ValidateModelListFromMaster(request, InputTransactionType.RQ_SUBMITTED);

            Assert.False(result);
        }

        [Fact]
        public async Task WhenPartNoAndWarehouseLocationNoAreMatchedInMaster_ButSomeUomIsNull_ShouldReturnFalse()
        {
            var mapper = new InvetoryRequestPartToInventoryRequestMapper();
            Mock<IMasterClient> mockClient = new Mock<IMasterClient>();
            mockClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(new PartModel
            {
                Data = new List<PartModel.DataModel> {
                new PartModel.DataModel { No = "p1", UomId = null },
                new PartModel.DataModel { No = "p2", UomId = new PartModel.UomModel { Name = "uom2" } }
            }
            }));
            mockClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(new WarehouseLocationModel
            {
                Data = new List<WarehouseLocationModel.DataModel> {
                new WarehouseLocationModel.DataModel { Code = "w1"  }
            }
            }));
            var request = new Domain.DTO.Requests.InventoryRequest
            {
                WarehouseLocationNo = "w1",
                Parts = new List<Domain.DTO.Requests.Part>{
                    new Domain.DTO.Requests.Part
                    {
                        No = "p1", Uom = "uom3"
                    },
                    new Domain.DTO.Requests.Part
                    {
                        No = "p2", Uom = "uom2"
                    },
                }
            };
            var service = CreateInventoryService(mockClient.Object, mapper);

            var result = await service.ValidateModelListFromMaster(request, InputTransactionType.RQ_SUBMITTED);

            Assert.False(result);
        }

        [Fact]
        public async Task WhenPartNoAndWarehouseLocationNoAreMatchedInMaster_ButSomeUomIsEmpty_ShouldReturnFalse()
        {
            var mapper = new InvetoryRequestPartToInventoryRequestMapper();
            Mock<IMasterClient> mockClient = new Mock<IMasterClient>();
            mockClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(new PartModel
            {
                Data = new List<PartModel.DataModel> {
                new PartModel.DataModel { No = "p1", UomId = new PartModel.UomModel() },
                new PartModel.DataModel { No = "p2", UomId = new PartModel.UomModel { Name = "uom2" } }
            }
            }));
            mockClient.Setup(x => x.GetWarehouseNoAsync()).Returns(Task.FromResult(new WarehouseLocationModel
            {
                Data = new List<WarehouseLocationModel.DataModel> {
                new WarehouseLocationModel.DataModel { Code = "w1"  }
            }
            }));
            var request = new Domain.DTO.Requests.InventoryRequest
            {
                WarehouseLocationNo = "w1",
                Parts = new List<Domain.DTO.Requests.Part>{
                    new Domain.DTO.Requests.Part
                    {
                        No = "p1", Uom = "uom3"
                    },
                    new Domain.DTO.Requests.Part
                    {
                        No = "p2", Uom = "uom2"
                    },
                }
            };
            var service = CreateInventoryService(mockClient.Object, mapper);

            var result = await service.ValidateModelListFromMaster(request, InputTransactionType.RQ_SUBMITTED);

            Assert.False(result);
        }
    }
}
