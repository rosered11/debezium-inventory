using System.Net;

namespace InventoryManagement.Api
{
    public class ApplicationResponse
    {
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
    }
    public class ApplicationResponseBuilder
    {
        public static ApplicationResponse Ok()
        {
            return new ApplicationResponse { StatusCode = Api.StatusCode.Ok, StatusMessage = Api.StatusMessage.Ok, HttpStatusCode = HttpStatusCode.OK };
        }

        public static ApplicationResponse BadRequest()
        {
            return new ApplicationResponse { StatusCode = Api.StatusCode.BadRequest, StatusMessage = Api.StatusMessage.BadRequest, HttpStatusCode = HttpStatusCode.BadRequest };
        }

        public static ApplicationResponse Unauthorized()
        {
            return new ApplicationResponse { StatusCode = Api.StatusCode.Unauthorized, StatusMessage = Api.StatusMessage.Unauthorized, HttpStatusCode = HttpStatusCode.Unauthorized };
        }

        public static ApplicationResponse Forbidden()
        {
            return new ApplicationResponse { StatusCode = Api.StatusCode.Forbidden, StatusMessage = Api.StatusMessage.Forbidden, HttpStatusCode = HttpStatusCode.Forbidden };
        }

        public static ApplicationResponse NotFound()
        {
            return new ApplicationResponse { StatusCode = Api.StatusCode.NotFound, StatusMessage = Api.StatusMessage.NotFound, HttpStatusCode = HttpStatusCode.NotFound };
        }

        public static ApplicationResponse InternalServerError()
        {
            return new ApplicationResponse { StatusCode = Api.StatusCode.InternalServerError, StatusMessage = Api.StatusMessage.InternalServerError, HttpStatusCode = HttpStatusCode.InternalServerError };
        }

        public static ApplicationResponse BadRequest(string statusCode)
        {
            switch(statusCode)
            {
                case StatusCode.BadRequest40010:
                    return new ApplicationResponse { StatusCode = Api.StatusCode.BadRequest40010, StatusMessage = Api.StatusMessage.BadRequest40010, HttpStatusCode = HttpStatusCode.BadRequest };
            }
            return BadRequest();
        }
    }
}
