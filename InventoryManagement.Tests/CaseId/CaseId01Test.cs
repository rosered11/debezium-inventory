using InventoryManagement.Domain.DTO.Requests;
using InventoryManagement.Domain.Utilities;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace InventoryManagement.Tests.CaseId
{
    public class CaseId01Test
    {
        #region Check validate key partNo
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void WhenFieldPartNoHasNotValue_ShouldReturnFalse(string input)
        {
            var payload = new Inventory
            {
                PartNo = input,
                Quantity = 1,
                TransactionType = nameof(InputTransactionType.RC_ACCEPTED),
                WarehouseLocationNo = "wh"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.PartNo);

            Assert.False(result);
        }

        [Fact]
        public void WhenFieldPartNoHasValue_ShouldReturnTrue()
        {
            var payload = new Inventory
            {
                PartNo = "p1",
                Quantity = 1,
                TransactionType = nameof(InputTransactionType.RC_ACCEPTED),
                WarehouseLocationNo = "wh"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.PartNo);

            Assert.True(result);
        }
        #endregion

        #region Check validate key Quantity

        [Fact]
        public void WhenFieldQuantity_TypeInt_HasDefaultValue_ShouldReturnTrue()
        {
            var payload = new Inventory
            {
                PartNo = "p1",
                Quantity = default,
                TransactionType = nameof(InputTransactionType.RC_ACCEPTED),
                WarehouseLocationNo = "wh"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.Quantity);

            Assert.True(result);
        }

        [Fact]
        public void WhenFieldQuantity_TypeInt_HasValue_ShouldReturnTrue()
        {
            var payload = new Inventory
            {
                PartNo = "p1",
                Quantity = 1,
                TransactionType = nameof(InputTransactionType.RC_ACCEPTED),
                WarehouseLocationNo = "wh"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.Quantity);

            Assert.True(result);
        }

        #endregion

        #region Check validate key TransactionType

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void WhenFieldTransactionTypeHasNotValue_ShouldReturnFalse(string input)
        {
            var payload = new Inventory
            {
                PartNo = "p1",
                Quantity = 1,
                TransactionType = input,
                WarehouseLocationNo = "wh"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.TransactionType);

            Assert.False(result);
        }

        [Fact]
        public void WhenFieldTransactionTypeHasValue_ShouldReturnTrue()
        {
            var payload = new Inventory
            {
                PartNo = "p1",
                Quantity = 1,
                TransactionType = nameof(InputTransactionType.RC_ACCEPTED),
                WarehouseLocationNo = "wh"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.TransactionType);

            Assert.True(result);
        }

        #endregion

        #region Check validate key WarehouseLocationNo

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void WhenFieldWarehouseLocationNoHasNotValue_ShouldReturnFalse(string input)
        {
            var payload = new Inventory
            {
                PartNo = "p1",
                Quantity = 1,
                TransactionType = nameof(InputTransactionType.RC_ACCEPTED),
                WarehouseLocationNo = input
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.WarehouseLocationNo);

            Assert.False(result);
        }

        [Fact]
        public void WhenFieldWarehouseLocationNoHasValue_ShouldReturnTrue()
        {
            var payload = new Inventory
            {
                PartNo = "p1",
                Quantity = 1,
                TransactionType = nameof(InputTransactionType.RC_ACCEPTED),
                WarehouseLocationNo = "wh"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.WarehouseLocationNo);

            Assert.True(result);
        }

        #endregion

        #region Check validate key uom

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void WhenFieldUomHasNotValue_ShouldReturnFalse(string input)
        {
            var payload = new Inventory
            {
                PartNo = "p1",
                Quantity = 1,
                TransactionType = nameof(InputTransactionType.RC_ACCEPTED),
                WarehouseLocationNo = input,
                Uom = input
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.WarehouseLocationNo);

            Assert.False(result);
        }

        [Fact]
        public void WhenFieldUomHasValue_ShouldReturnTrue()
        {
            var payload = new Inventory
            {
                PartNo = "p1",
                Quantity = 1,
                TransactionType = nameof(InputTransactionType.RC_ACCEPTED),
                WarehouseLocationNo = "wh",
                Uom = "uom"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.WarehouseLocationNo);

            Assert.True(result);
        }

        #endregion
    }
}
