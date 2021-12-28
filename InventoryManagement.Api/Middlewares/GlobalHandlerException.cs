using Boxed.Mapping;
using InventoryManagement.Domain.DTO.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace InventoryManagement.Api
{
    public class GlobalHandlerException
    {
        private readonly RequestDelegate _next;

        public GlobalHandlerException(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context
            , IMapper<ApplicationResponse, Domain.DTO.Responses.BaseResponse> mapper
            , ILogger<GlobalHandlerException> logger
            , IMapper<Domain.DTO.Models.AvailableOfPathModel, BadRequestForRequestValidationResponse.PartModel> availableOfPathMapper
            , IMapper<ApplicationResponse, Domain.DTO.Responses.BadRequestForRequestValidationResponse> badRequestApplicationResponseMapper)
        {
            try
            {
                await _next(context);
                if (context.Response.StatusCode.Equals((int)HttpStatusCode.Unauthorized))
                {
                    await ReWriteAsync(context, mapper, () => ApplicationResponseBuilder.Unauthorized());
                }

                if(context.Response.StatusCode.Equals((int)HttpStatusCode.Forbidden))
                {
                    await ReWriteAsync(context, mapper, () => ApplicationResponseBuilder.Forbidden());
                }

                if(context.Response.StatusCode.Equals((int)HttpStatusCode.NotFound))
                {
                    await ReWriteAsync(context, mapper, () => ApplicationResponseBuilder.NotFound());
                }

                if(context.Response.StatusCode.ToString().Equals(StatusCode.BadRequest40010))
                {
                    var applicationResponse = ApplicationResponseBuilder.BadRequest(StatusCode.BadRequest40010);
                    var collectionValidate = (IList<Domain.DTO.Models.AvailableOfPathModel>)context.Items["collection_validate"];
                    var badRequestResponse = badRequestApplicationResponseMapper.Map(applicationResponse);
                    badRequestResponse.Result.Parts = availableOfPathMapper.MapList(collectionValidate);

                    var response = context.Response;
                    response.ContentType = "application/json";
                    context.Response.StatusCode = (int)applicationResponse.HttpStatusCode;

                    await response.WriteAsync(JsonSerializer.Serialize(badRequestResponse));
                }
            }
            catch (Exception error)
            {
                logger.LogError(error, "Middleware catch exception.");
                await ReWriteAsync(context, mapper, () => ApplicationResponseBuilder.InternalServerError());
            }
        }

        private async Task ReWriteAsync(HttpContext context, IMapper<ApplicationResponse, Domain.DTO.Responses.BaseResponse> mapper, Func<ApplicationResponse> getInstantBuilder)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            var applicationResponse = getInstantBuilder();
            context.Response.StatusCode = (int)applicationResponse.HttpStatusCode;
            var baseResponse = mapper.Map(applicationResponse);
            await response.WriteAsync(JsonSerializer.Serialize(baseResponse));
        }
    }
}
