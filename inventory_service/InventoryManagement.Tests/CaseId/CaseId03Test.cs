using InventoryManagement.Domain.DTO.Requests;
using InventoryManagement.Domain.DTO.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace InventoryManagement.Tests.CaseId
{
    public class CaseId03Test
    {
        #region Check validate key RequestOrderNo
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void WhenFieldRequestOrderNoHasNotValue_ShouldReturnFalse(string input)
        {
            var payload = new InventoryRequest
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
                RequestOrderNo = input
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.RequestOrderNo);

            Assert.False(result);
        }

        [Fact]
        public void WhenFieldRequestOrderNoHasValue_ShouldReturnTrue()
        {
            var payload = new InventoryRequest
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
                RequestOrderNo = "ab1"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.RequestOrderNo);

            Assert.True(result);
        }
        #endregion

        #region Check validate key WarehouseLocationNo
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void WhenFieldWarehouseLocationNoHasNotValue_ShouldReturnFalse(string input)
        {
            var payload = new InventoryRequest
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
                RequestOrderNo = "re1"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.WarehouseLocationNo);

            Assert.False(result);
        }

        [Fact]
        public void WhenFieldWarehouseLocationNoHasValue_ShouldReturnTrue()
        {
            var payload = new InventoryRequest
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
                RequestOrderNo = "ab1"
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
            var payload = new InventoryRequest
            {
                Parts = null,
                WarehouseLocationNo = "wh1",
                RequestOrderNo = "re1"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.Parts);

            Assert.False(result);
        }

        [Fact]
        public void WhenFieldPartsHasValue_ShouldReturnTrue()
        {
            var payload = new InventoryRequest
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
                RequestOrderNo = "ab1"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.Parts);

            Assert.True(result);
        }

        [Fact]
        public void WhenFieldPartsIsEmptyList_ShoudReturnFalse()
        {
            var payload = new InventoryRequest
            {
                Parts = new List<Part>(),
                WarehouseLocationNo = "wh",
                RequestOrderNo = "ab1"
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
            var payload = new InventoryRequest
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
                RequestOrderNo = "re1"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.Parts.First().No);

            Assert.False(result);
        }

        [Fact]
        public void WhenFieldNoInSectionPartsHasValue_ShouldReturnTrue()
        {
            var payload = new InventoryRequest
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
                RequestOrderNo = "ab1"
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
            var payload = new InventoryRequest
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
                RequestOrderNo = "re1"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.Parts.First().Qty);

            Assert.True(result);
        }

        [Fact]
        public void WhenFieldQtyInSectionPartsHasValue_ShouldReturnTrue()
        {
            var payload = new InventoryRequest
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
                RequestOrderNo = "ab1"
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
            var payload = new InventoryRequest
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
                RequestOrderNo = "re1"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.Parts.First().Uom);

            Assert.False(result);
        }

        [Fact]
        public void WhenFieldUomInSectionPartsHasValue_ShouldReturnTrue()
        {
            var payload = new InventoryRequest
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
                RequestOrderNo = "ab1"
            };

            var attributeValidate = new RequiredAttribute();

            var result = attributeValidate.IsValid(payload.Parts.First().Uom);

            Assert.True(result);
        }
        #endregion
    }
}
