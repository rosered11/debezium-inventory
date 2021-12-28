using InventoryManagement.Domain.Services;
using InventoryManagement.Domain.Utilities;
using System.Collections.Generic;
using Xunit;

namespace InventoryManagement.Tests.Services.OutboxEventServiceTest
{
    public class CheckAvailableQuantityTest
    {
        private static OutboxEventService CreateOutboxEventService()
            => new OutboxEventService(null, null, null, null);

        #region CheckAvailableQuantity for request part

        [Theory]
        [InlineData(InventoryTransactionType.STOCK_BOOKED)]
        [InlineData(InventoryTransactionType.STOCK_DECREASED)]
        [InlineData(InventoryTransactionType.STOCK_INCOMING)]
        [InlineData(InventoryTransactionType.STOCK_INCREASED)]
        [InlineData(InventoryTransactionType.STOCK_RECEIVING)]
        public void WhenQuantityRequest_LessThenInventoryAvailable_ShouldNotChangeTransactionType(InventoryTransactionType type)
        {
            var service = CreateOutboxEventService();
            var outboxEvent = new Infrastructure.Entities.OutboxEvent
            {
                Type = type.ToString()
            };
            var request = new Domain.DTO.Requests.Inventory
            {
                Quantity = 2
            };
            var currentInventory = new Infrastructure.Entities.Inventory
            {
                AvailableQty = 3
            };

            var result = service.CheckAvailableQuantity(outboxEvent, request, currentInventory);

            Assert.NotNull(result);
            Assert.Equal(type.ToString(), result.Type);
        }

        [Theory]
        [InlineData(InventoryTransactionType.STOCK_BOOKED)]
        [InlineData(InventoryTransactionType.STOCK_DECREASED)]
        [InlineData(InventoryTransactionType.STOCK_INCOMING)]
        [InlineData(InventoryTransactionType.STOCK_INCREASED)]
        [InlineData(InventoryTransactionType.STOCK_RECEIVING)]
        public void WhenQuantityRequest_EqualInventoryAvailable_ShouldNotChangeTransactionType(InventoryTransactionType type)
        {
            var service = CreateOutboxEventService();
            var outboxEvent = new Infrastructure.Entities.OutboxEvent
            {
                Type = type.ToString()
            };
            var request = new Domain.DTO.Requests.Inventory
            {
                Quantity = 3
            };
            var inventory = new Infrastructure.Entities.Inventory
            {
                AvailableQty = 3
            };

            var result = service.CheckAvailableQuantity(outboxEvent, request, inventory);

            Assert.NotNull(result);
            Assert.Equal(type.ToString(), result.Type);
        }

        [Theory]
        [InlineData(InventoryTransactionType.STOCK_DECREASED)]
        [InlineData(InventoryTransactionType.STOCK_INCOMING)]
        [InlineData(InventoryTransactionType.STOCK_INCREASED)]
        [InlineData(InventoryTransactionType.STOCK_RECEIVING)]
        public void WhenQuantityRequest_MoreThenInventoryAvailable_AndInventoryTypeWithoutBooked_ShouldNotChangeTransactionType(InventoryTransactionType type)
        {
            var service = CreateOutboxEventService();
            var outboxEvent = new Infrastructure.Entities.OutboxEvent
            {
                Type = type.ToString()
            };
            var request = new Domain.DTO.Requests.Inventory
            {
                Quantity = 4
            };
            var inventory = new Infrastructure.Entities.Inventory
            {
                AvailableQty = 3
            };

            var result = service.CheckAvailableQuantity(outboxEvent, request, inventory);

            Assert.NotNull(result);
            Assert.Equal(type.ToString(), result.Type);
        }

        [Fact]
        public void WhenQuantityRequest_MoreThenInventoryAvailable_AndInventoryTypeIsBooked_ShouldChangeTransactionTypeToBookedFail()
        {
            var service = CreateOutboxEventService();
            var outboxEvent = new Infrastructure.Entities.OutboxEvent
            {
                Type = "STOCK_BOOKED"
            };
            var request = new Domain.DTO.Requests.Inventory
            {
                Quantity = 4
            };
            var inventory = new Infrastructure.Entities.Inventory
            {
                AvailableQty = 3
            };

            var result = service.CheckAvailableQuantity(outboxEvent, request, inventory);

            Assert.NotNull(result);
            Assert.Equal("STOCK_BOOKING_FAILED", result.Type);
        }

        [Fact]
        public void WhenCurrentInventoryIsNull_AndInventoryTypeIsBooked_ShouldChangeTransactionTypeToBookedFail()
        {
            var service = CreateOutboxEventService();
            var outboxEvent = new Infrastructure.Entities.OutboxEvent
            {
                Type = "STOCK_BOOKED"
            };
            var request = new Domain.DTO.Requests.Inventory
            {
                Quantity = 4
            };
            Infrastructure.Entities.Inventory currentInventory = null;

            var result = service.CheckAvailableQuantity(outboxEvent, request, currentInventory);

            Assert.NotNull(result);
            Assert.Equal("STOCK_BOOKING_FAILED", result.Type);
        }

        [Fact]
        public void WhenCurrentInventoryIsEmpty_AndInventoryTypeIsBooked_ShouldChangeTransactionTypeToBookedFail()
        {
            var service = CreateOutboxEventService();
            var outboxEvent = new Infrastructure.Entities.OutboxEvent
            {
                Type = "STOCK_BOOKED"
            };
            var request = new Domain.DTO.Requests.Inventory
            {
                Quantity = 4
            };
            Infrastructure.Entities.Inventory currentInventory = new Infrastructure.Entities.Inventory();

            var result = service.CheckAvailableQuantity(outboxEvent, request, currentInventory);

            Assert.NotNull(result);
            Assert.Equal("STOCK_BOOKING_FAILED", result.Type);
        }
        #endregion


        #region CheckAvailableQuantity for request order

        [Theory]
        [InlineData(InventoryTransactionType.STOCK_BOOKED)]
        [InlineData(InventoryTransactionType.STOCK_DECREASED)]
        [InlineData(InventoryTransactionType.STOCK_INCOMING)]
        [InlineData(InventoryTransactionType.STOCK_INCREASED)]
        [InlineData(InventoryTransactionType.STOCK_RECEIVING)]
        public void WhenQuantityRequestForRequestOrder_LessThenInventoryAvailable_ShouldNotChangeTransactionType(InventoryTransactionType type)
        {
            //OutboxEvent outboxEvent, DTO.Requests.InventoryRequest request, IEnumerable<Inventory> currentInventoryList)
            var service = CreateOutboxEventService();
            var outboxEvent = new Infrastructure.Entities.OutboxEvent
            {
                Type = type.ToString()
            };
            var request = new Domain.DTO.Requests.InventoryRequest
            {
                Parts = new List<Domain.DTO.Requests.Part> { 
                    new Domain.DTO.Requests.Part { 
                        No = "p1",
                        Qty = 2
                    } 
                }
            };
            var currentInventoryList = new List<Infrastructure.Entities.Inventory> {
                new Infrastructure.Entities.Inventory
                {
                    PartNo = "p1",
                    AvailableQty = 3
                },
                new Infrastructure.Entities.Inventory
                {
                    PartNo = "p2",
                    AvailableQty = 2
                }
            };

            var result = service.CheckAvailableQuantity(outboxEvent, request, currentInventoryList);

            Assert.NotNull(result);
            Assert.Equal(type.ToString(), result.Type);
        }

        [Theory]
        [InlineData(InventoryTransactionType.STOCK_BOOKED)]
        [InlineData(InventoryTransactionType.STOCK_DECREASED)]
        [InlineData(InventoryTransactionType.STOCK_INCOMING)]
        [InlineData(InventoryTransactionType.STOCK_INCREASED)]
        [InlineData(InventoryTransactionType.STOCK_RECEIVING)]
        public void WhenQuantityRequestForRequestOrder_EqualInventoryAvailable_ShouldNotChangeTransactionType(InventoryTransactionType type)
        {
            var service = CreateOutboxEventService();
            var outboxEvent = new Infrastructure.Entities.OutboxEvent
            {
                Type = type.ToString()
            };
            var request = new Domain.DTO.Requests.InventoryRequest
            {
                Parts = new List<Domain.DTO.Requests.Part> {
                    new Domain.DTO.Requests.Part {
                        No = "p1",
                        Qty = 3
                    }
                }
            };
            var currentInventoryList = new List<Infrastructure.Entities.Inventory> {
                new Infrastructure.Entities.Inventory
                {
                    PartNo = "p1",
                    AvailableQty = 3
                },
                new Infrastructure.Entities.Inventory
                {
                    PartNo = "p2",
                    AvailableQty = 2
                }
            };

            var result = service.CheckAvailableQuantity(outboxEvent, request, currentInventoryList);

            Assert.NotNull(result);
            Assert.Equal(type.ToString(), result.Type);
        }

        [Theory]
        [InlineData(InventoryTransactionType.STOCK_DECREASED)]
        [InlineData(InventoryTransactionType.STOCK_INCOMING)]
        [InlineData(InventoryTransactionType.STOCK_INCREASED)]
        [InlineData(InventoryTransactionType.STOCK_RECEIVING)]
        public void WhenQuantityRequestForRequestOrder_MoreThenInventoryAvailable_AndInventoryTypeWithoutBooked_ShouldNotChangeTransactionType(InventoryTransactionType type)
        {
            var service = CreateOutboxEventService();
            var outboxEvent = new Infrastructure.Entities.OutboxEvent
            {
                Type = type.ToString()
            };
            var request = new Domain.DTO.Requests.InventoryRequest
            {
                Parts = new List<Domain.DTO.Requests.Part> {
                    new Domain.DTO.Requests.Part {
                        No = "p1",
                        Qty = 4
                    }
                }
            };
            var currentInventoryList = new List<Infrastructure.Entities.Inventory> {
                new Infrastructure.Entities.Inventory
                {
                    PartNo = "p1",
                    AvailableQty = 3
                },
                new Infrastructure.Entities.Inventory
                {
                    PartNo = "p2",
                    AvailableQty = 2
                }
            };

            var result = service.CheckAvailableQuantity(outboxEvent, request, currentInventoryList);

            Assert.NotNull(result);
            Assert.Equal(type.ToString(), result.Type);
        }

        [Fact]
        public void WhenQuantityRequestForRequestOrder_MoreThenInventoryAvailable_AndInventoryTypeIsBooked_ShouldChangeTransactionTypeToBookedFail()
        {
            var service = CreateOutboxEventService();
            var outboxEvent = new Infrastructure.Entities.OutboxEvent
            {
                Type = "STOCK_BOOKED"
            };
            var request = new Domain.DTO.Requests.InventoryRequest
            {
                Parts = new List<Domain.DTO.Requests.Part> {
                    new Domain.DTO.Requests.Part {
                        No = "p1",
                        Qty = 4
                    }
                }
            };
            var currentInventoryList = new List<Infrastructure.Entities.Inventory> {
                new Infrastructure.Entities.Inventory
                {
                    PartNo = "p1",
                    AvailableQty = 3
                },
                new Infrastructure.Entities.Inventory
                {
                    PartNo = "p2",
                    AvailableQty = 2
                }
            };

            var result = service.CheckAvailableQuantity(outboxEvent, request, currentInventoryList);

            Assert.NotNull(result);
            Assert.Equal("STOCK_BOOKING_FAILED", result.Type);
        }

        [Fact]
        public void WhenCurrentInventoryIsNullForRequestOrder_AndInventoryTypeIsBooked_ShouldChangeTransactionTypeToBookedFail()
        {
            var service = CreateOutboxEventService();
            var outboxEvent = new Infrastructure.Entities.OutboxEvent
            {
                Type = "STOCK_BOOKED"
            };
            var request = new Domain.DTO.Requests.InventoryRequest
            {
                Parts = new List<Domain.DTO.Requests.Part> {
                    new Domain.DTO.Requests.Part {
                        No = "p1",
                        Qty = 4
                    }
                }
            };
            List<Infrastructure.Entities.Inventory> currentInventoryList = null;

            var result = service.CheckAvailableQuantity(outboxEvent, request, currentInventoryList);

            Assert.NotNull(result);
            Assert.Equal("STOCK_BOOKING_FAILED", result.Type);
        }

        [Fact]
        public void WhenCurrentInventoryIsEmptyForRequestOrder_AndInventoryTypeIsBooked_ShouldChangeTransactionTypeToBookedFail()
        {
            var service = CreateOutboxEventService();
            var outboxEvent = new Infrastructure.Entities.OutboxEvent
            {
                Type = "STOCK_BOOKED"
            };
            var request = new Domain.DTO.Requests.InventoryRequest
            {
                Parts = new List<Domain.DTO.Requests.Part> {
                    new Domain.DTO.Requests.Part {
                        No = "p1",
                        Qty = 4
                    }
                }
            };
            List<Infrastructure.Entities.Inventory> currentInventoryList = new List<Infrastructure.Entities.Inventory>();

            var result = service.CheckAvailableQuantity(outboxEvent, request, currentInventoryList);

            Assert.NotNull(result);
            Assert.Equal("STOCK_BOOKING_FAILED", result.Type);
        }

        #endregion
    }
}
