using Boxed.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InventoryManagement.Api
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var svc = context.HttpContext.RequestServices;
                var mapper = (IMapper<ApplicationResponse, Domain.DTO.Responses.BaseResponse>)
                    svc.GetService(typeof(IMapper<ApplicationResponse, Domain.DTO.Responses.BaseResponse>));

                var applicationResponse = ApplicationResponseBuilder.BadRequest();
                var baseResponse = mapper.Map(applicationResponse);
                var badReqResponse = new BadRequestObjectResult(baseResponse);
                badReqResponse.StatusCode = (int)applicationResponse.HttpStatusCode;

                context.Result = badReqResponse;
            }
        }
    }
}
