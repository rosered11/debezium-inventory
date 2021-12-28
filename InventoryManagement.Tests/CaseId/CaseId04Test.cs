using InventoryManagement.Domain.DTO.Requests;
using InventoryManagement.Domain.DTO.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace InventoryManagement.Tests.CaseId
{
    public class CaseId04Test
    {
        #region Check validate key ReceiveOrderNo
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void WhenFieldReceiveOrderNoHasNotValue_ShouldReturnFalse(string input)
        {
            var payload = new ReceiveEvent
            {
                Parts = new List<Part>
                {
                    new Part
                    {
                        No = "no1",
                        Qty = 5,
                        Uom = "uom"
                    }
                },
                WarehouseLocationNo = "wh",
                ReceiveOrderNo = input
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.ReceiveOrderNo);

            Assert.False(result);
        }

        [Fact]
        public void WhenFieldReceiveOrderNoHasValue_ShouldReturnTrue()
        {
            var payload = new ReceiveEvent
            {
                Parts = new List<Part>
                {
                    new Part
                    {
                        No = "no1",
                        Qty = 5,
                        Uom = "uom"
                    }
                },
                WarehouseLocationNo = "wh",
                ReceiveOrderNo = "ab1"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.ReceiveOrderNo);

            Assert.True(result);
        }
        #endregion

        #region Check validate key WarehouseLocationNo
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void WhenFieldWarehouseLocationNoHasNotValue_ShouldReturnFalse(string input)
        {
            var payload = new ReceiveEvent
            {
                Parts = new List<Part>
                {
                    new Part
                    {
                        No = "no1",
                        Qty = 5,
                        Uom = "uom"
                    }
                },
                WarehouseLocationNo = input,
                ReceiveOrderNo = "re1"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.WarehouseLocationNo);

            Assert.False(result);
        }

        [Fact]
        public void WhenFieldWarehouseLocationNoHasValue_ShouldReturnTrue()
        {
            var payload = new ReceiveEvent
            {
                Parts = new List<Part>
                {
                    new Part
                    {
                        No = "no1",
                        Qty = 5,
                        Uom = "uom"
                    }
                },
                WarehouseLocationNo = "wh",
                ReceiveOrderNo = "ab1"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.WarehouseLocationNo);

            Assert.True(result);
        }
        #endregion

        #region Check validate key Parts
        [Fact]
        public void WhenFieldPartsHasNotValue_ShouldReturnFalse()
        {
            var payload = new ReceiveEvent
            {
                Parts = null,
                WarehouseLocationNo = "wh1",
                ReceiveOrderNo = "re1"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.Parts);

            Assert.False(result);
        }

        [Fact]
        public void WhenFieldPartsHasValue_ShouldReturnTrue()
        {
            var payload = new ReceiveEvent
            {
                Parts = new List<Part>
                {
                    new Part
                    {
                        No = "no1",
                        Qty = 5,
                        Uom = "uom"
                    }
                },
                WarehouseLocationNo = "wh",
                ReceiveOrderNo = "ab1"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.Parts);

            Assert.True(result);
        }

        [Fact]
        public void WhenFieldPartsIsEmptyList_ShoudReturnFalse()
        {
            var payload = new ReceiveEvent
            {
                Parts = new List<Part>(),
                WarehouseLocationNo = "wh",
                ReceiveOrderNo = "ab1"
            };

            var attributeValidate = new ValidateListAttribute();

            var result = attributeValidate.IsValid(payload.Parts);

            Assert.False(result);
        }

        #endregion

        #region Check validate key No in section Parts
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void WhenFieldNoInSectionPartsHasNotValue_ShouldReturnFalse(string input)
        {
            var payload = new ReceiveEvent
            {
                Parts = new List<Part>
                {
                    new Part
                    {
                        No = input,
                        Qty = 5,
                        Uom = "uom"
                    }
                },
                WarehouseLocationNo = "wh1",
                ReceiveOrderNo = "re1"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.Parts.First().No);

            Assert.False(result);
        }

        [Fact]
        public void WhenFieldNoInSectionPartsHasValue_ShouldReturnTrue()
        {
            var payload = new ReceiveEvent
            {
                Parts = new List<Part>
                {
                    new Part
                    {
                        No = "no1",
                        Qty = 5,
                        Uom = "uom"
                    }
                },
                WarehouseLocationNo = "wh",
                ReceiveOrderNo = "ab1"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.Parts.First().No);

            Assert.True(result);
        }
        #endregion

        #region Check validate key Qty in section Parts
        [Fact]
        public void WhenFieldQtyInSectionPartsIsDefault_ShouldReturnTrue()
        {
            var payload = new ReceiveEvent
            {
                Parts = new List<Part>
                {
                    new Part
                    {
                        No = "no1",
                        Qty = default,
                        Uom = "uom"
                    }
                },
                WarehouseLocationNo = "wh1",
                ReceiveOrderNo = "re1"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.Parts.First().Qty);

            Assert.True(result);
        }

        [Fact]
        public void WhenFieldQtyInSectionPartsHasValue_ShouldReturnTrue()
        {
            var payload = new ReceiveEvent
            {
                Parts = new List<Part>
                {
                    new Part
                    {
                        No = "no1",
                        Qty = 5,
                        Uom = "uom"
                    }
                },
                WarehouseLocationNo = "wh",
                ReceiveOrderNo = "ab1"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.Parts.First().Qty);

            Assert.True(result);
        }
        #endregion

        #region Check validate key Uom in section Parts

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void WhenFieldUomInSectionPartsIsHasNotValue_ShouldReturnFalse(string input)
        {
            var payload = new ReceiveEvent
            {
                Parts = new List<Part>
                {
                    new Part
                    {
                        No = "no1",
                        Qty = 5,
                        Uom = input
                    }
                },
                WarehouseLocationNo = "wh1",
                ReceiveOrderNo = "re1"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.Parts.First().Uom);

            Assert.False(result);
        }

        [Fact]
        public void WhenFieldUomInSectionPartsHasValue_ShouldReturnTrue()
        {
            var payload = new ReceiveEvent
            {
                Parts = new List<Part>
                {
                    new Part
                    {
                        No = "no1",
                        Qty = 5,
                        Uom = "uom"
                    }
                },
                WarehouseLocationNo = "wh",
                ReceiveOrderNo = "ab1"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.Parts.First().Uom);

            Assert.True(result);
        }
        #endregion

        #region Check validate key Event Type

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void WhenFieldEventTypeHasNotValue_ShouldReturnFalse(string input)
        {
            var payload = new ReceiveEvent
            {
                Parts = new List<Part>
                {
                    new Part
                    {
                        No = "no1",
                        Qty = 5,
                        Uom = input
                    }
                },
                WarehouseLocationNo = "wh1",
                ReceiveOrderNo = "re1",
                EventType = input
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.EventType);

            Assert.False(result);
        }

        [Fact]
        public void WhenFieldEventTypeHasValue_ShouldReturnTrue()
        {
            var payload = new ReceiveEvent
            {
                Parts = new List<Part>
                {
                    new Part
                    {
                        No = "no1",
                        Qty = 5,
                        Uom = "uom"
                    }
                },
                WarehouseLocationNo = "wh",
                ReceiveOrderNo = "ab1",
                EventType = "eventype"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.EventType);

            Assert.True(result);
        }
        #endregion

    }
}
