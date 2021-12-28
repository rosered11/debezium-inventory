using Boxed.Mapping;
using InventoryManagement.Domain.Mapper;
using InventoryManagement.Domain.Services;
using InventoryManagement.Domain.Utilities;
using InventoryManagement.Infrastructure.Entities;
using Xunit;

namespace InventoryManagement.Tests.Services.OutboxEventServiceTest
{
    public class CreateOutboxEventPerPartTest
    {
        private static OutboxEventService CreateOutboxEventService(IMapper<Domain.DTO.Requests.Inventory, Infrastructure.Entities.OutboxEvent> mapper) 
            => new OutboxEventService(mapper, null, null, null);

        [Theory]
        [InlineData(InputTransactionType.RC_SUBMITTED, InventoryTransactionType.STOCK_INCOMING, "ReceivePart")]
        [InlineData(InputTransactionType.RC_ACCEPTED, InventoryTransactionType.STOCK_RECEIVING, "ReceivePart")]
        [InlineData(InputTransactionType.RC_COMPLETED, InventoryTransactionType.STOCK_INCREASED, "ReceivePart")]
        [InlineData(InputTransactionType.RQ_SUBMITTED, InventoryTransactionType.STOCK_BOOKED, "RequestPart")]
        [InlineData(InputTransactionType.RQ_DELIVERED, InventoryTransactionType.STOCK_DECREASED, "RequestPart")]
        public void WhenCreateOutboxEventPerPart_ShouldReturnOubboxEventIsCorrect(InputTransactionType inputType, InventoryTransactionType inventoryType, string expectAggregateType)
        {
            var mapper = new OutboxEventMapper();
            var request = new Domain.DTO.Requests.Inventory
            {
                PartNo = "p1",
                Quantity = 3,
                TransactionType = inputType.ToString(),
                WarehouseLocationNo = "wh"
            };

            var inventory = new Infrastructure.Entities.Inventory
            {
                AvailableQty = 3
            };

            var service = CreateOutboxEventService(mapper);

            var result = service.CreateOutboxEventPerPart(request, inventory);

            Assert.Equal("p1", result.AggregateId);
            Assert.Equal(expectAggregateType, result.AggregateType);
            Assert.Equal(inventoryType.ToString(), result.Type);
        }
    }
}
