using InventoryManagement.Api;
using System.Net;
using Xunit;

namespace InventoryManagement.Tests
{
    public class ApplicationReponseBuilderTest
    {
        [Fact]
        public void WhenCreateApplicationResponseStatusOk_ShouldReturnIsCorrect()
        {
            var result = ApplicationResponseBuilder.Ok();

            Assert.Equal("Success", result.StatusMessage);
            Assert.Equal("00000", result.StatusCode);
            Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
        }

        [Fact]
        public void WhenCreateApplicationResponseStatusBadRequest_ShouldReturnIsCorrect()
        {
            var result = ApplicationResponseBuilder.BadRequest();

            Assert.Equal("Invalid Request", result.StatusMessage);
            Assert.Equal("40000", result.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, result.HttpStatusCode);
        }

        [Fact]
        public void WhenCreateApplicationResponseStatusUnauthorized_ShouldReturnIsCorrect()
        {
            var result = ApplicationResponseBuilder.Unauthorized();

            Assert.Equal("Unauthorized Access", result.StatusMessage);
            Assert.Equal("40100", result.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, result.HttpStatusCode);
        }

        [Fact]
        public void WhenCreateApplicationResponseStatusForbidden_ShouldReturnIsCorrect()
        {
            var result = ApplicationResponseBuilder.Forbidden();

            Assert.Equal("Access Forbidden", result.StatusMessage);
            Assert.Equal("40300", result.StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, result.HttpStatusCode);
        }

        [Fact]
        public void WhenCreateApplicationResponseStatusNotFound_ShouldReturnIsCorrect()
        {
            var result = ApplicationResponseBuilder.NotFound();

            Assert.Equal("Data Not Found", result.StatusMessage);
            Assert.Equal("40400", result.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, result.HttpStatusCode);
        }

        [Fact]
        public void WhenCreateApplicationResponseStatusInternalServerError_ShouldReturnIsCorrect()
        {
            var result = ApplicationResponseBuilder.InternalServerError();

            Assert.Equal("Internal Server Error", result.StatusMessage);
            Assert.Equal("50000", result.StatusCode);
            Assert.Equal(HttpStatusCode.InternalServerError, result.HttpStatusCode);
        }

        [Fact]
        public void WhenCreateApplicationResponseStatusBadRequest40010_ShouldReturnIsCorrect()
        {
            var result = ApplicationResponseBuilder.BadRequest(StatusCode.BadRequest40010);

            Assert.Equal("Some parts have not enough stocks", result.StatusMessage);
            Assert.Equal("40010", result.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, result.HttpStatusCode);
        }
    }
}
