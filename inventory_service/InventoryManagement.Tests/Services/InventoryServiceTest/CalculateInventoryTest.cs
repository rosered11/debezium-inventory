using Boxed.Mapping;
using InventoryManagement.Domain.Mapper;
using InventoryManagement.Domain.Services;
using InventoryManagement.Domain.Utilities;
using InventoryManagement.Infrastructure.Entities;
using Xunit;

namespace InventoryManagement.Tests.Services.InventoryServiceTest
{
    public class CalculateInventoryTest
    {
        private static InventoryService CreateInventoryService()
            => new InventoryService(null, null, null, null, null, null);

        [Fact]
        public void WhenTransactionTypeIsRoSubmitted_AndWithoutCurrentInventory_ShouldReturnInventoryFollowRequest()
        {
            var service = CreateInventoryService();
            var request = new Domain.DTO.Requests.Inventory
            {
                PartNo = "mockPartNo",
                Quantity = 10,
                WarehouseLocationNo = "wh1",
                TransactionType = nameof(InputTransactionType.RC_SUBMITTED),
                Uom = "uom"
            };

            var result = service.CalculateInventory(request, null);

            Assert.Equal("mockPartNo", result.PartNo);
            Assert.Equal("wh1", result.WarehouseLocationNo);
            Assert.Equal(10, result.POQty);
            Assert.Equal(0, result.ReceivingQty);
            Assert.Equal(0, result.BalanceQty);
            Assert.Equal(0, result.RequestQty);
            Assert.Equal(0, result.AvailableQty);
            Assert.Equal("uom", result.Uom);
        }

        [Fact]
        public void WhenTransactionTypeIsRoSubmitted_ShouldReturnInventoryValueIsCorrect()
        {
            var service = CreateInventoryService();
            var currentInventory = new Inventory
            {
                PartNo = "mockPartNo",
                POQty = 5,
                WarehouseLocationNo = "wharehouse",
                Uom = "uom"
            };
            var request = new Domain.DTO.Requests.Inventory
            {
                PartNo = "mockPartNo",
                Quantity = 10,
                WarehouseLocationNo = "wharehouse",
                TransactionType = nameof(InputTransactionType.RC_SUBMITTED),
                Uom = "uom"
            };

            var result = service.CalculateInventory(request, currentInventory);

            Assert.Equal("mockPartNo", result.PartNo);
            Assert.Equal("wharehouse", result.WarehouseLocationNo);
            Assert.Equal(15, result.POQty);
            Assert.Equal(0, result.ReceivingQty);
            Assert.Equal(0, result.BalanceQty);
            Assert.Equal(0, result.RequestQty);
            Assert.Equal(0, result.AvailableQty);
            Assert.Equal("uom", result.Uom);
        }

        [Fact]
        public void WhenTransactionTypeIsRoAccepted_ShouldReturnInventoryValueIsCorrect()
        {
            var service = CreateInventoryService();
            var currentInventory = new Inventory
            {
                PartNo = "mockPartNo",
                POQty = 10,
                WarehouseLocationNo = "wharehouse",
                Uom = "uom"
            };
            var request = new Domain.DTO.Requests.Inventory
            {
                PartNo = "mockPartNo",
                Quantity = 10,
                WarehouseLocationNo = "wharehouse",
                TransactionType = nameof(InputTransactionType.RC_ACCEPTED),
                Uom = "uom"
            };

            var result = service.CalculateInventory(request, currentInventory);

            Assert.Equal("mockPartNo", result.PartNo);
            Assert.Equal("wharehouse", result.WarehouseLocationNo);
            Assert.Equal(0, result.POQty);
            Assert.Equal(10, result.ReceivingQty);
            Assert.Equal(0, result.BalanceQty);
            Assert.Equal(0, result.RequestQty);
            Assert.Equal(0, result.AvailableQty);
            Assert.Equal("uom", result.Uom);
        }

        [Fact]
        public void WhenTransactionTypeIsRoCompleted_ShouldReturnInventoryValueIsCorrect()
        {
            var service = CreateInventoryService();
            var currentInventory = new Inventory
            {
                PartNo = "mockPartNo",
                POQty = 10,
                ReceivingQty = 10,
                WarehouseLocationNo = "wharehouse",
                Uom = "uom"
            };
            var request = new Domain.DTO.Requests.Inventory
            {
                PartNo = "mockPartNo",
                Quantity = 5,
                WarehouseLocationNo = "wharehouse",
                TransactionType = nameof(InputTransactionType.RC_COMPLETED),
                Uom = "uom"
            };

            var result = service.CalculateInventory(request, currentInventory);

            Assert.Equal("mockPartNo", result.PartNo);
            Assert.Equal("wharehouse", result.WarehouseLocationNo);
            Assert.Equal(10, result.POQty);
            Assert.Equal(5, result.ReceivingQty);
            Assert.Equal(5, result.BalanceQty);
            Assert.Equal(0, result.RequestQty);
            Assert.Equal(5, result.AvailableQty);
            Assert.Equal("uom", result.Uom);
        }

        [Fact]
        public void WhenTransactionTypeIsRqAccepted_ShouldReturnInventoryValueIsCorrect()
        {
            var service = CreateInventoryService();
            var currentInventory = new Inventory
            {
                PartNo = "mockPartNo",
                POQty = 10,
                ReceivingQty = 10,
                BalanceQty = 5,
                WarehouseLocationNo = "wharehouse",
                Uom = "uom"
            };
            var request = new Domain.DTO.Requests.Inventory
            {
                PartNo = "mockPartNo",
                Quantity = 2,
                WarehouseLocationNo = "wharehouse",
                TransactionType = nameof(InputTransactionType.RQ_SUBMITTED),
                Uom = "uom"
            };

            var result = service.CalculateInventory(request, currentInventory);

            Assert.Equal("mockPartNo", result.PartNo);
            Assert.Equal("wharehouse", result.WarehouseLocationNo);
            Assert.Equal(10, result.POQty);
            Assert.Equal(10, result.ReceivingQty);
            Assert.Equal(5, result.BalanceQty);
            Assert.Equal(2, result.RequestQty);
            Assert.Equal(3, result.AvailableQty);
            Assert.Equal("uom", result.Uom);
        }

        [Fact]
        public void WhenTransactionTypeIsRqClosed_ShouldReturnInventoryValueIsCorrect()
        {
            var service = CreateInventoryService();
            var currentInventory = new Inventory
            {
                PartNo = "mockPartNo",
                POQty = 10,
                ReceivingQty = 10,
                BalanceQty = 5,
                RequestQty = 2,
                WarehouseLocationNo = "wharehouse",
                Uom = "uom"
            };
            var request = new Domain.DTO.Requests.Inventory
            {
                PartNo = "mockPartNo",
                Quantity = 2,
                WarehouseLocationNo = "wharehouse",
                TransactionType = nameof(InputTransactionType.RQ_DELIVERED),
                Uom = "uom"
            };

            var result = service.CalculateInventory(request, currentInventory);

            Assert.Equal("mockPartNo", result.PartNo);
            Assert.Equal("wharehouse", result.WarehouseLocationNo);
            Assert.Equal(10, result.POQty);
            Assert.Equal(10, result.ReceivingQty);
            Assert.Equal(3, result.BalanceQty);
            Assert.Equal(0, result.RequestQty);
            Assert.Equal(3, result.AvailableQty);
            Assert.Equal("uom", result.Uom);
        }
    }
}
