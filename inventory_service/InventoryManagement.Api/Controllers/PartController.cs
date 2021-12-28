using Boxed.Mapping;
using InventoryManagement.Domain.DTO.Responses;
using InventoryManagement.Domain.Services;
using InventoryManagement.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;
using System;
using System.Threading.Tasks;

namespace InventoryManagement.Api.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    [Authorize]
    public class PartController : Controller
    {
        private readonly IMapper<ApplicationResponse, BaseResponse> _mapper;
        private readonly PartService _partService;

        public PartController(IMapper<ApplicationResponse, Domain.DTO.Responses.BaseResponse> mapper
            , PartService partService)
        {
            _mapper = mapper;
            _partService = partService;
        }
        [HttpGet]
        public async Task<IActionResult> GetPartsHaveAvailableQty([FromQuery] SieveModel sieveModel)
        {
            var response = await _partService.GetPartsThatHaveAvailableQtyInStock(sieveModel);

            var applicationResponse = ApplicationResponseBuilder.Ok();
            var baseResponse = _mapper.Map(applicationResponse);
            response.StatusCode = baseResponse.StatusCode;
            response.StatusMessage = baseResponse.StatusMessage;
            return StatusCode((int)applicationResponse.HttpStatusCode, response);
        }

        [HttpGet]
        [Route("wh/{warehouseLocationNo}")]
        public async Task<IActionResult> GetPartsHaveAvailableQtyViaWarehouseLocationNo([FromQuery] SieveModel sieveModel, string warehouseLocationNo)
        {
            var response = await _partService.GetPartsThatHaveAvailableQtyInStock(sieveModel, warehouseLocationNo);

            var applicationResponse = ApplicationResponseBuilder.Ok();
            var baseResponse = _mapper.Map(applicationResponse);
            response.StatusCode = baseResponse.StatusCode;
            response.StatusMessage = baseResponse.StatusMessage;
            return StatusCode((int)applicationResponse.HttpStatusCode, response);
        }

        [HttpGet]
        [Route("type/{quantityType}")]
        public async Task<IActionResult> GetPartsByType([FromQuery] SieveModel sieveModel, string quantityType)
        {
            ApplicationResponse applicationResponse;
            BaseResponse baseResponse;

            // validate quantity type 
            if (!Enum.TryParse(quantityType, ignoreCase: true, out QuantityTypeEnum quantityTypeEnum))
            {
                applicationResponse = ApplicationResponseBuilder.BadRequest();
                baseResponse = _mapper.Map(applicationResponse);
                return StatusCode((int)applicationResponse.HttpStatusCode, baseResponse);
            }

            var response = await _partService.GetPartsSpecificTypeQtyInStock(sieveModel, quantityTypeEnum);

            applicationResponse = ApplicationResponseBuilder.Ok();
            baseResponse = _mapper.Map(applicationResponse);
            response.StatusCode = baseResponse.StatusCode;
            response.StatusMessage = baseResponse.StatusMessage;
            return StatusCode((int)applicationResponse.HttpStatusCode, response);
        }

        [HttpGet]
        [Route("wh/{warehouseLocationNo}/type/{quantityType}")]
        public async Task<IActionResult> GetPartsSpecificTypeQtyViaWarehouseLocationNo([FromQuery] SieveModel sieveModel, string warehouseLocationNo, string quantityType)
        {
            ApplicationResponse applicationResponse;
            BaseResponse baseResponse;

            // validate quantity type 
            if (!Enum.TryParse(quantityType, ignoreCase: true, out QuantityTypeEnum quantityTypeEnum))
            {
                applicationResponse = ApplicationResponseBuilder.BadRequest();
                baseResponse = _mapper.Map(applicationResponse);
                return StatusCode((int)applicationResponse.HttpStatusCode, baseResponse);
            }

            var response = await _partService.GetPartsSpecificTypeQtyInStock(sieveModel, warehouseLocationNo, quantityTypeEnum);

            applicationResponse = ApplicationResponseBuilder.Ok();
            baseResponse = _mapper.Map(applicationResponse);
            response.StatusCode = baseResponse.StatusCode;
            response.StatusMessage = baseResponse.StatusMessage;
            return StatusCode((int)applicationResponse.HttpStatusCode, response);
        }
    }
}
