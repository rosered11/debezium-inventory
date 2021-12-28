using Boxed.Mapping;
using InventoryManagement.Domain.Mapper;
using InventoryManagement.Domain.Services;
using InventoryManagement.Domain.Utilities;
using System.Collections.Generic;
using Xunit;

namespace InventoryManagement.Tests.Services.OutboxEventServiceTest
{
    public class CreateOutboxEventPerOrderTest
    {
        private static OutboxEventService CreateOutboxEventService(IMapper<Domain.DTO.Requests.InventoryRequest
            , Infrastructure.Entities.OutboxEvent> mapper
            , IMapper<Domain.DTO.Requests.ReceiveEvent, Infrastructure.Entities.OutboxEvent> receiveMapper
            , IMapper<Domain.DTO.Requests.RequestEvent, Infrastructure.Entities.OutboxEvent> requestMapper)
            => new OutboxEventService(null, mapper, receiveMapper, requestMapper);

        [Theory]
        [InlineData(InputTransactionType.RC_SUBMITTED, InventoryTransactionType.STOCK_INCOMING, "ReceiveOrder")]
        [InlineData(InputTransactionType.RC_ACCEPTED, InventoryTransactionType.STOCK_RECEIVING, "ReceiveOrder")]
        [InlineData(InputTransactionType.RC_COMPLETED, InventoryTransactionType.STOCK_INCREASED, "ReceiveOrder")]
        [InlineData(InputTransactionType.RQ_SUBMITTED, InventoryTransactionType.STOCK_BOOKED, "RequestOrder")]
        [InlineData(InputTransactionType.RQ_DELIVERED, InventoryTransactionType.STOCK_DECREASED, "RequestOrder")]
        public void WhenCreateOutboxEventByInventoryRequest_ShouldReturnOubboxEventIsCorrect(InputTransactionType inputType, InventoryTransactionType inventoryType, string expectAggregateType)
        {
            var mapper = new OutboxEventForRequestInventory();
            var receiveMapper = new ReceiveEventToOutboxEventMapper();
            var request = new Domain.DTO.Requests.InventoryRequest
            {
                Parts = new List<Domain.DTO.Requests.Part> {
                    new Domain.DTO.Requests.Part
                    {
                        No = "p1",
                        Qty = 3,
                        Uom = "uom"
                    }
                },
                WarehouseLocationNo = "wh",
                RequestOrderNo = "requestOrder"
            };

            var inventoryList = new List<Infrastructure.Entities.Inventory> {
                new Infrastructure.Entities.Inventory
                {
                    AvailableQty = 3,
                    PartNo = "p1"
                }
            };

            var service = CreateOutboxEventService(mapper, receiveMapper, null);

            var result = service.CreateOutboxEventPerOrder(request, inputType, request.RequestOrderNo);

            Assert.Equal("requestOrder", result.AggregateId);
            Assert.Equal(expectAggregateType, result.AggregateType);
            Assert.Equal(inventoryType.ToString(), result.Type);
        }

        [Theory]
        [InlineData(InputTransactionType.RC_SUBMITTED, InventoryTransactionType.STOCK_INCOMING, "ReceiveOrder")]
        [InlineData(InputTransactionType.RC_ACCEPTED, InventoryTransactionType.STOCK_RECEIVING, "ReceiveOrder")]
        [InlineData(InputTransactionType.RC_COMPLETED, InventoryTransactionType.STOCK_INCREASED, "ReceiveOrder")]
        [InlineData(InputTransactionType.RQ_SUBMITTED, InventoryTransactionType.STOCK_BOOKED, "RequestOrder")]
        [InlineData(InputTransactionType.RQ_DELIVERED, InventoryTransactionType.STOCK_DECREASED, "RequestOrder")]
        public void WhenCreateOutboxEventByReceiveEventRequest_ShouldReturnOubboxEventIsCorrect(InputTransactionType inputType, InventoryTransactionType inventoryType, string expectAggregateType)
        {
            var mapper = new OutboxEventForRequestInventory();
            var receiveMapper = new ReceiveEventToOutboxEventMapper();
            var request = new Domain.DTO.Requests.ReceiveEvent
            {
                Parts = new List<Domain.DTO.Requests.Part> {
                    new Domain.DTO.Requests.Part
                    {
                        No = "p1",
                        Qty = 3,
                        Uom = "uom"
                    }
                },
                WarehouseLocationNo = "wh",
                ReceiveOrderNo = "receiveOrder"
            };

            var inventoryList = new List<Infrastructure.Entities.Inventory> {
                new Infrastructure.Entities.Inventory
                {
                    AvailableQty = 3,
                    PartNo = "p1"
                }
            };

            var service = CreateOutboxEventService(mapper, receiveMapper, null);

            var result = service.CreateOutboxEventPerOrder(request, inputType, request.ReceiveOrderNo);

            Assert.Equal("receiveOrder", result.AggregateId);
            Assert.Equal(expectAggregateType, result.AggregateType);
            Assert.Equal(inventoryType.ToString(), result.Type);
        }

        [Theory]
        [InlineData(InputTransactionType.RC_SUBMITTED, InventoryTransactionType.STOCK_INCOMING, "ReceiveOrder")]
        [InlineData(InputTransactionType.RC_ACCEPTED, InventoryTransactionType.STOCK_RECEIVING, "ReceiveOrder")]
        [InlineData(InputTransactionType.RC_COMPLETED, InventoryTransactionType.STOCK_INCREASED, "ReceiveOrder")]
        [InlineData(InputTransactionType.RQ_SUBMITTED, InventoryTransactionType.STOCK_BOOKED, "RequestOrder")]
        [InlineData(InputTransactionType.RQ_DELIVERED, InventoryTransactionType.STOCK_DECREASED, "RequestOrder")]
        public void WhenCreateOutboxEventByRequestEventRequest_ShouldReturnOubboxEventIsCorrect(InputTransactionType inputType, InventoryTransactionType inventoryType, string expectAggregateType)
        {
            var mapper = new OutboxEventForRequestInventory();
            var requestMapper = new RequestEventToOutboxEventMapper();
            var request = new Domain.DTO.Requests.RequestEvent
            {
                Parts = new List<Domain.DTO.Requests.Part> {
                    new Domain.DTO.Requests.Part
                    {
                        No = "p1",
                        Qty = 3,
                        Uom = "uom"
                    }
                },
                WarehouseLocationNo = "wh",
                RequestOrderNo = "requestOrder"
            };

            var inventoryList = new List<Infrastructure.Entities.Inventory> {
                new Infrastructure.Entities.Inventory
                {
                    AvailableQty = 3,
                    PartNo = "p1"
                }
            };

            var service = CreateOutboxEventService(mapper, null, requestMapper);

            var result = service.CreateOutboxEventPerOrder(request, inputType, request.RequestOrderNo);

            Assert.Equal("requestOrder", result.AggregateId);
            Assert.Equal(expectAggregateType, result.AggregateType);
            Assert.Equal(inventoryType.ToString(), result.Type);
        }
    }
}
