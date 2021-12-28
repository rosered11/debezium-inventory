using Boxed.Mapping;
using InventoryManagement.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;
using System.Threading.Tasks;

namespace InventoryManagement.Api.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    [Authorize]
    public class WarehouseController : ControllerBase
    {
        private readonly IMapper<ApplicationResponse, Domain.DTO.Responses.BaseResponse> _mapper;
        private readonly WarehouseService _warehouseService;

        public WarehouseController(
            IMapper<ApplicationResponse, Domain.DTO.Responses.BaseResponse> mapper
            , WarehouseService warehouseService)
        {
            _mapper = mapper;
            _warehouseService = warehouseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWarehouseLocation([FromQuery] SieveModel sieveModel)
        {
            var response = await _warehouseService.GetWarehouseLocationHasBalanceAsync(sieveModel);

            var applicationResponse = ApplicationResponseBuilder.Ok();
            var baseResponse = _mapper.Map(applicationResponse);
            response.StatusCode = baseResponse.StatusCode;
            response.StatusMessage = baseResponse.StatusMessage;
            return StatusCode((int)applicationResponse.HttpStatusCode, response);
        }
    }
}
