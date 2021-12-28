using Boxed.Mapping;

namespace InventoryManagement.Api.Mapper
{
    public class ApplicationResponseMapper : IMapper<ApplicationResponse, Domain.DTO.Responses.BaseResponse>
    {
        public void Map(ApplicationResponse source, Domain.DTO.Responses.BaseResponse destination)
        {
            destination.StatusCode = source.StatusCode;
            destination.StatusMessage = source.StatusMessage;
        }
    }

    public class ApplicationResponseToBadRequestForRequestValidateMapper : IMapper<ApplicationResponse, Domain.DTO.Responses.BadRequestForRequestValidationResponse>
    {
        public void Map(ApplicationResponse source, Domain.DTO.Responses.BadRequestForRequestValidationResponse destination)
        {
            destination.StatusCode = source.StatusCode;
            destination.StatusMessage = source.StatusMessage;
        }
    }
}
