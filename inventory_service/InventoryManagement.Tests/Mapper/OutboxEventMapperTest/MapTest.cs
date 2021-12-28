using Boxed.Mapping;
using InventoryManagement.Domain.DTO.Requests;
using InventoryManagement.Domain.Mapper;
using InventoryManagement.Domain.Utilities;
using Snapshooter.Xunit;
using System.Collections.Generic;
using Xunit;

namespace InventoryManagement.Tests.Mapper.OutboxEventMapperTest
{
    public class MapTest
    {
        [Fact]
        public void WhenMappingInventoryToOutboxEvent_ShouldReturnIsCorrect()
        {
            IMapper<Domain.DTO.Requests.Inventory, Infrastructure.Entities.OutboxEvent> service = new OutboxEventMapper();
            var input = new Inventory
            {
                PartNo = "p1",
                WarehouseLocationNo = "w1",
                Quantity = 2,
                TransactionType = nameof(InputTransactionType.RC_SUBMITTED),
                Uom = "uom"
            };

            var result = service.Map(input);

            Assert.Equal(nameof(InventoryTransactionType.STOCK_INCOMING), result.Type);
            Snapshot.Match(result.Payload);
        }

        [Fact]
        public void WhenMappingInventoryRequestToOutboxEvent_ShouldReturnIsCorrect()
        {
            IMapper<Domain.DTO.Requests.InventoryRequest, Infrastructure.Entities.OutboxEvent> service = new OutboxEventForRequestInventory();
            var input = new InventoryRequest
            {
                RequestOrderNo = "req1",
                WarehouseLocationNo = "wh",
                Parts = new List<Part>
                {
                    new Part
                    {
                        No = "n1",
                        Qty = 2,
                        Uom = "uom"
                    }
                }
            };

            var result = service.Map(input);

            Snapshot.Match(result.Payload);
        }

        [Fact]
        public void WhenMappingReceiveEventToOutboxEvent_ShouldReturnIsCorrect()
        {
            IMapper<Domain.DTO.Requests.ReceiveEvent, Infrastructure.Entities.OutboxEvent> service = new ReceiveEventToOutboxEventMapper();
            var input = new ReceiveEvent
            {
                ReceiveOrderNo = "req1",
                WarehouseLocationNo = "wh",
                EventType = "event",
                Parts = new List<Part>
                {
                    new Part
                    {
                        No = "n1",
                        Qty = 2,
                        Uom = "uom"
                    }
                }
            };

            var result = service.Map(input);

            Snapshot.Match(result.Payload);
        }

        [Fact]
        public void WhenMappingRequestEventToOutboxEvent_ShouldReturnIsCorrect()
        {
            IMapper<Domain.DTO.Requests.RequestEvent, Infrastructure.Entities.OutboxEvent> service = new RequestEventToOutboxEventMapper();
            var input = new RequestEvent
            {
                RequestOrderNo = "req2",
                WarehouseLocationNo = "wh",
                EventType = "event2",
                Parts = new List<Part>
                {
                    new Part
                    {
                        No = "n1",
                        Qty = 2,
                        Uom = "uom"
                    }
                }
            };

            var result = service.Map(input);

            Snapshot.Match(result.Payload);
        }
    }
}
