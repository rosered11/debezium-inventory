using Boxed.Mapping;
using InventoryManagement.Api;
using InventoryManagement.Api.Mapper;
using Xunit;

namespace InventoryManagement.Tests.Mapper.ApplicationResponseMapperTest
{
    public class MapTest
    {
        [Fact]
        public void WhenMappingApplicationResponseToBaseResponse_ShouldReturnIsCorrect()
        {
            IMapper<ApplicationResponse, Domain.DTO.Responses.BaseResponse> service = new ApplicationResponseMapper();
            var input = new ApplicationResponse
            {
                StatusCode = "1",
                StatusMessage = "message"
            };

            var result = service.Map(input);

            Assert.Equal("1", result.StatusCode);
            Assert.Equal("message", result.StatusMessage);
        }
    }
}
