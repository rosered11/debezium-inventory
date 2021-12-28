using InventoryManagement.Domain.DTO.Requests;
using InventoryManagement.Domain.DTO.Validation;
using InventoryManagement.Domain.Utilities;
using Xunit;

namespace InventoryManagement.Tests.CaseId
{
    public class CaseId02Test
    {
        [Theory]
        [InlineData(InputTransactionType.RC_ACCEPTED)]
        [InlineData(InputTransactionType.RC_COMPLETED)]
        [InlineData(InputTransactionType.RC_SUBMITTED)]
        [InlineData(InputTransactionType.RQ_DELIVERED)]
        [InlineData(InputTransactionType.RQ_SUBMITTED)]
        public void WhenValueOfTransactionTypeIsCorrect_ShouldReturnTrue(InputTransactionType type)
        {
            var payload = new Inventory
            {
                PartNo = "p1",
                Quantity = default,
                TransactionType = type.ToString(),
                WarehouseLocationNo = "wh"
            };

            var attributeValidate = new ValidateEnumAttribute();

            var result = attributeValidate.IsValid(payload.TransactionType);

            Assert.True(result);
        }

        [Fact]
        public void WhenValueOfTransactionTypeIsNotCorrect_ShouldReturnFalse()
        {
            var payload = new Inventory
            {
                PartNo = "p1",
                Quantity = 1,
                TransactionType = "abc123",
                WarehouseLocationNo = "wh"
            };

            var attributeValidate = new ValidateEnumAttribute();

            var result = attributeValidate.IsValid(payload.TransactionType);

            Assert.False(result);
        }
    }
}
