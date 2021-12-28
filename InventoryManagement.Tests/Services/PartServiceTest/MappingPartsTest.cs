using InventoryManagement.Domain.Client;
using InventoryManagement.Domain.Client.Models.MasterClient;
using InventoryManagement.Domain.Services;
using InventoryManagement.Infrastructure.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace InventoryManagement.Tests.Services.PartServiceTest
{
    public class MappingPartsTest
    {
        private PartService CreatePartService(IMasterClient masterClient) => new PartService(null, masterClient);
        // Have masterdata and has parts
        [Fact]
        public async Task WhenMappingMasterdataWithParts_ShouldReturnPartsAreCorrect()
        {
            var masterClient = new Mock<IMasterClient>();
            masterClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(new PartModel
            {
                Data = new List<PartModel.DataModel> {
                    new PartModel.DataModel
                    {
                        No = "p1",
                        Name = "partname1"
                    },
                    new PartModel.DataModel
                    {
                        No = "p2",
                        Name = "partname2"
                    }
                }
            }));
            var inventory = new List<Inventory>
            {
                new Inventory
                {
                    PartNo = "p1",
                    AvailableQty = 1,
                    Uom = "uom1"
                },
                new Inventory
                {
                    PartNo = "p2",
                    AvailableQty = 2,
                    Uom = "uom2"
                }
            };
            var service = CreatePartService(masterClient.Object);

            var result = await service.MappingPartsAsync(inventory.AsQueryable());

            Assert.Equal(2, result.Count());
            var part = result.FirstOrDefault(x => x.PartNo == "p1");
            Assert.Equal("partname1", part.PartName);
            Assert.Equal("uom1", part.Uom);
            Assert.Equal(1, part.Available);
            part = result.FirstOrDefault(x => x.PartNo == "p2");
            Assert.Equal("partname2", part.PartName);
            Assert.Equal("uom2", part.Uom);
            Assert.Equal(2, part.Available);
        }

        // have parts but don't have master
        [Fact]
        public async Task WhenMappingParts_ButMasterdataInSectionDataIsEmpty_ShouldReturnPartsAreCorrect()
        {
            var masterClient = new Mock<IMasterClient>();
            masterClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(new PartModel
            {
                Data = null
            }));
            var inventory = new List<Inventory>
            {
                new Inventory
                {
                    PartNo = "p1",
                    AvailableQty = 1,
                    Uom = "uom1"
                },
                new Inventory
                {
                    PartNo = "p2",
                    AvailableQty = 2,
                    Uom = "uom2"
                }
            };
            var service = CreatePartService(masterClient.Object);

            var result = await service.MappingPartsAsync(inventory.AsQueryable());

            Assert.Equal(2, result.Count());
            var part = result.FirstOrDefault(x => x.PartNo == "p1");
            Assert.Null(part.PartName);
            Assert.Equal("uom1", part.Uom);
            Assert.Equal(1, part.Available);
            part = result.FirstOrDefault(x => x.PartNo == "p2");
            Assert.Null(part.PartName);
            Assert.Equal("uom2", part.Uom);
            Assert.Equal(2, part.Available);
        }

        [Fact]
        public async Task WhenMappingParts_ButMasterdataIsEmpty_ShouldReturnPartsAreCorrect()
        {
            var masterClient = new Mock<IMasterClient>();
            masterClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(new PartModel()));
            var inventory = new List<Inventory>
            {
                new Inventory
                {
                    PartNo = "p1",
                    AvailableQty = 1,
                    Uom = "uom1"
                },
                new Inventory
                {
                    PartNo = "p2",
                    AvailableQty = 2,
                    Uom = "uom2"
                }
            };
            var service = CreatePartService(masterClient.Object);

            var result = await service.MappingPartsAsync(inventory.AsQueryable());

            Assert.Equal(2, result.Count());
            var part = result.FirstOrDefault(x => x.PartNo == "p1");
            Assert.Null(part.PartName);
            Assert.Equal("uom1", part.Uom);
            Assert.Equal(1, part.Available);
            part = result.FirstOrDefault(x => x.PartNo == "p2");
            Assert.Null(part.PartName);
            Assert.Equal("uom2", part.Uom);
            Assert.Equal(2, part.Available);
        }

        [Fact]
        public async Task WhenMappingParts_ButMasterdataIsNull_ShouldReturnPartsAreCorrect()
        {
            var masterClient = new Mock<IMasterClient>();
            masterClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(default(PartModel)));
            var inventory = new List<Inventory>
            {
                new Inventory
                {
                    PartNo = "p1",
                    AvailableQty = 1,
                    Uom = "uom1"
                },
                new Inventory
                {
                    PartNo = "p2",
                    AvailableQty = 2,
                    Uom = "uom2"
                }
            };
            var service = CreatePartService(masterClient.Object);

            var result = await service.MappingPartsAsync(inventory.AsQueryable());

            Assert.Equal(2, result.Count());
            var part = result.FirstOrDefault(x => x.PartNo == "p1");
            Assert.Null(part.PartName);
            Assert.Equal("uom1", part.Uom);
            Assert.Equal(1, part.Available);
            part = result.FirstOrDefault(x => x.PartNo == "p2");
            Assert.Null(part.PartName);
            Assert.Equal("uom2", part.Uom);
            Assert.Equal(2, part.Available);
        }

        // dont have parts
        [Fact]
        public async Task WhenMappingMasterdataWithParts_ButDontHavePart_ShouldReturnPartsAreCorrect()
        {
            var masterClient = new Mock<IMasterClient>();
            masterClient.Setup(x => x.GetPartsAsync()).Returns(Task.FromResult(new PartModel
            {
                Data = new List<PartModel.DataModel> {
                    new PartModel.DataModel
                    {
                        No = "p1",
                        Name = "partname1"
                    },
                    new PartModel.DataModel
                    {
                        No = "p2",
                        Name = "partname2"
                    }
                }
            }));
            var inventory = new List<Inventory>();
            var service = CreatePartService(masterClient.Object);

            var result = await service.MappingPartsAsync(inventory.AsQueryable());

            Assert.Empty(result);
        }
    }
}
