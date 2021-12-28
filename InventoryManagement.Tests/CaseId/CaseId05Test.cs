using InventoryManagement.Domain.DTO.Requests;
using InventoryManagement.Domain.DTO.Validation;
using InventoryManagement.Domain.Utilities;
using Xunit;

namespace InventoryManagement.Tests.CaseId
{
    public class CaseId05Test
    {
        public class CaseId02Test
        {
            [Theory]
            [InlineData(InputTransactionType.RC_ACCEPTED)]
            [InlineData(InputTransactionType.RC_COMPLETED)]
            [InlineData(InputTransactionType.RC_SUBMITTED)]
            public void WhenValueOfReceiveEventTypeIsCorrect_ShouldReturnTrue(InputTransactionType type)
            {
                var payload = new ReceiveEvent
                {
                    EventType = type.ToString()
                };

                var attributeValidate = new ValidateEnumAttribute(nameof(ValidateEnumType.Receive));

                var result = attributeValidate.IsValid(payload.EventType);

                Assert.True(result);
            }

            [Theory]
            [InlineData(InputTransactionType.RQ_DELIVERED)]
            [InlineData(InputTransactionType.RQ_SUBMITTED)]
            public void WhenValueOfReceiveEventTypeIsNotCorrect_ShouldReturnFalse(InputTransactionType type)
            {
                var payload = new ReceiveEvent
                {
                    EventType = type.ToString()
                };

                var attributeValidate = new ValidateEnumAttribute(nameof(ValidateEnumType.Receive));

                var result = attributeValidate.IsValid(payload.EventType);

                Assert.False(result);
            }
        }
    }
}
