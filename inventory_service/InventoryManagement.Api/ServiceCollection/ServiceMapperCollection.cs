using Boxed.Mapping;
using InventoryManagement.Api.Mapper;
using InventoryManagement.Domain.Mapper;
using InventoryManagement.Infrastructure.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagement.Api
{
    public static class ServiceMapperCollection
    {
        public static IServiceCollection AddMappers(this IServiceCollection services)
        {
            services.AddSingleton<IMapper<Inventory, Domain.DTO.Responses.Row>, InventoryToInventoryRowResponseMapper>();
            services.AddSingleton<IMapper<ApplicationResponse, Domain.DTO.Responses.BaseResponse>, ApplicationResponseMapper>();
            services.AddSingleton<IMapper<ApplicationResponse, Domain.DTO.Responses.BadRequestForRequestValidationResponse>, ApplicationResponseToBadRequestForRequestValidateMapper>();
            services.AddSingleton<IMapper<Domain.DTO.Models.AvailableOfPathModel, Domain.DTO.Responses.BadRequestForRequestValidationResponse.PartModel>, AvailableOfPathModelMapper>();
            services.AddSingleton<IMapper<Domain.DTO.Requests.Part, Domain.DTO.Requests.Inventory>, InvetoryRequestPartToInventoryRequestMapper>();
            services.AddSingleton<IMapper<Domain.DTO.Requests.Inventory, Infrastructure.Entities.OutboxEvent>, OutboxEventMapper>();
            services.AddSingleton<IMapper<Domain.DTO.Requests.InventoryRequest, Infrastructure.Entities.OutboxEvent>, OutboxEventForRequestInventory>();
            services.AddSingleton<IMapper<Domain.DTO.Requests.ReceiveEvent, Domain.DTO.Requests.InventoryRequest>, ReceiveEventReqToInventoryRequestMapper>();
            services.AddSingleton<IMapper<Domain.DTO.Requests.Part, Domain.DTO.Requests.Part>, ReceiveEventPartReqToInventoryRequestPartMapper>();
            services.AddSingleton<IMapper<Domain.DTO.Requests.ReceiveEvent, Infrastructure.Entities.OutboxEvent>, ReceiveEventToOutboxEventMapper>();
            services.AddSingleton<IMapper<Domain.DTO.Requests.RequestEvent, Domain.DTO.Requests.InventoryRequest>, RequestEventReqToInventoryRequestMapper>();
            services.AddSingleton<IMapper<Domain.DTO.Requests.RequestEvent, Infrastructure.Entities.OutboxEvent>, RequestEventToOutboxEventMapper>();
            return services;
        }
    }
}
