using Boxed.Mapping;
using InventoryManagement.Api.Authentication;
using InventoryManagement.Domain.Services;
using InventoryManagement.Domain.Utilities;
using InventoryManagement.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sieve.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryService _inventoryService;
        private readonly IMapper<ApplicationResponse, Domain.DTO.Responses.BaseResponse> _mapper;
        private readonly IMapper<Domain.DTO.Requests.ReceiveEvent, Domain.DTO.Requests.InventoryRequest> _eventReceiveMapper;
        private readonly OutboxEventService _outboxEventService;
        private readonly UnitOfWork _unitOfWork;
        private readonly ILogger<InventoryController> _logger;
        private readonly IMapper<Domain.DTO.Requests.RequestEvent, Domain.DTO.Requests.InventoryRequest> _eventRequestMapper;

        public InventoryController(InventoryService inventoryService
            , IMapper<ApplicationResponse, Domain.DTO.Responses.BaseResponse> mapper
            , IMapper<Domain.DTO.Requests.ReceiveEvent, Domain.DTO.Requests.InventoryRequest> eventReceiveMapper
            , OutboxEventService outboxEventService
            , UnitOfWork unitOfWork
            , ILogger<InventoryController> logger
            , IMapper<Domain.DTO.Requests.RequestEvent, Domain.DTO.Requests.InventoryRequest> eventRequestMapper)
        {
            _inventoryService = inventoryService;
            _mapper = mapper;
            _eventReceiveMapper = eventReceiveMapper;
            _outboxEventService = outboxEventService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _eventRequestMapper = eventRequestMapper;
        }

        [Authorize]
        [HttpPost, Route("AdjustPartQuantity")]
        [ValidateModel]
        public async Task<IActionResult> AdjustPartQuantity([FromBody] Domain.DTO.Requests.Inventory payload)
        {
            ApplicationResponse applicationResponse;
            Domain.DTO.Responses.BaseResponse baseResponse;
            bool isFound = await _inventoryService.ValidateModelFromMaster(payload);
            if (!isFound)
            {
                applicationResponse = ApplicationResponseBuilder.NotFound();
                return StatusCode((int)applicationResponse.HttpStatusCode, string.Empty);
            }
            else
            {
                await _inventoryService.AdjustPartQuantityAsync(payload);
                applicationResponse = ApplicationResponseBuilder.Ok();
            }
            
            baseResponse = _mapper.Map(applicationResponse);
            return StatusCode((int)applicationResponse.HttpStatusCode, baseResponse);
        }

        [Authorize]
        [HttpGet]
        [Route("GetStockInventory")]
        [Route("GetStockInventory/wh/{warehouseLocationNo}")]
        [Route("GetStockInventory/wh/{warehouseLocationNo}/part/{partNo}")]
        [Route("GetStockInventory/part/{partNo}")]
        public async Task<IActionResult> GetStockInventory([FromQuery] SieveModel sieveModel, string warehouseLocationNo, string partNo)
        {
            var response = await _inventoryService.GetStockInventoryAsync(sieveModel, warehouseLocationNo, partNo);
            var applicationResponse = ApplicationResponseBuilder.Ok();
            var baseResponse = _mapper.Map(applicationResponse);
            response.StatusCode = baseResponse.StatusCode;
            response.StatusMessage = baseResponse.StatusMessage;
            return StatusCode((int)applicationResponse.HttpStatusCode, response);
        }

        [Authorize]
        [HttpPost, Route("request/validation")]
        [ValidateModel]
        public async Task<IActionResult> RequestValidation([FromBody] Domain.DTO.Requests.InventoryRequest request)
        {
            // sum quantity for same part no
            request.Parts = _inventoryService.SumQuantityForSamePartNo(request.Parts);

            // validate quantity 
            var validateCollection = await _inventoryService.ValidateQuantity(request);
            if(validateCollection.Any(x => !x.IsEnough))
            {
                HttpContext.Items["collection_validate"] = validateCollection;
                return StatusCode(Convert.ToInt32(Api.StatusCode.BadRequest40010), string.Empty);
            }

            var applicationResponse = ApplicationResponseBuilder.Ok();
            var baseResponse = _mapper.Map(applicationResponse);
            return StatusCode((int)applicationResponse.HttpStatusCode, baseResponse);
        }

        [Authorize]
        [HttpPost, Route("request/allocate")]
        [ValidateModel]
        public async Task<IActionResult> RequestAllocate([FromBody] Domain.DTO.Requests.InventoryRequest request)
        {
            // Check warehouse location no and partno on master
            ApplicationResponse applicationResponse;
            Domain.DTO.Responses.BaseResponse baseResponse;
            bool isFound = await _inventoryService.ValidateModelListFromMaster(request, Domain.Utilities.InputTransactionType.RQ_SUBMITTED);
            if (!isFound)
            {
                applicationResponse = ApplicationResponseBuilder.NotFound();
                return StatusCode((int)applicationResponse.HttpStatusCode, string.Empty);
            }

            // sum quantity for same part no
            request.Parts = _inventoryService.SumQuantityForSamePartNo(request.Parts);

            // validate quantity 
            var validateCollection = await _inventoryService.ValidateQuantity(request);
            if (validateCollection.Any(x => !x.IsEnough))
            {
                HttpContext.Items["collection_validate"] = validateCollection;
                return StatusCode(Convert.ToInt32(Api.StatusCode.BadRequest40010), string.Empty);
            }

            var outboxEvent = _outboxEventService.CreateOutboxEventPerOrder(request, Domain.Utilities.InputTransactionType.RQ_SUBMITTED, request.RequestOrderNo);
            await _inventoryService.AdjustPartsQuantityAsync(request, Domain.Utilities.InputTransactionType.RQ_SUBMITTED, outboxEvent);
            applicationResponse = ApplicationResponseBuilder.Ok();
            baseResponse = _mapper.Map(applicationResponse);
            return StatusCode((int)applicationResponse.HttpStatusCode, baseResponse);
        }

        [Authorize(AuthenticationSchemes = AuthenConstant.DefaultScheme)]
        [HttpPost, Route("receive/event-handler")]
        [ValidateModel]
        public async Task<IActionResult> ReceiveEventHandler([FromBody] Domain.DTO.Requests.ReceiveEvent request)
        {
            ApplicationResponse applicationResponse;
            Domain.DTO.Responses.BaseResponse baseResponse;

            var eventId = Request.Headers.GetValueHeader(EventConstant.EVENT_ID);
            var eventType = Request.Headers.GetValueHeader(EventConstant.EVENT_TYPE);

            // validate event id and event type must be values
            if(string.IsNullOrEmpty(eventId) || string.IsNullOrEmpty(eventType))
            {
                _logger.LogWarning("The event id or event type isn't value.");
                applicationResponse = ApplicationResponseBuilder.BadRequest();
                baseResponse = _mapper.Map(applicationResponse);
                return StatusCode((int)applicationResponse.HttpStatusCode, baseResponse);
            }

            // check idempotent
            if(_unitOfWork.IdempotentRepository.Find(x => x.EventId == eventId && x.EventType == eventType).Any())
            {
                _logger.LogInformation($"These even id: \"{eventId}\" and event type: \"{eventType}\" has been in the system.");
                applicationResponse = ApplicationResponseBuilder.Ok();
                baseResponse = _mapper.Map(applicationResponse);
                return StatusCode((int)applicationResponse.HttpStatusCode, baseResponse);
            }

            // Check warehouse location no and partno on master
            var inventoryRequest = _eventReceiveMapper.Map(request);
            bool isFound = await _inventoryService.ValidateModelListFromMaster(inventoryRequest, Enum.Parse<Domain.Utilities.InputTransactionType>(request.EventType));
            if (!isFound)
            {
                applicationResponse = ApplicationResponseBuilder.NotFound();
                return StatusCode((int)applicationResponse.HttpStatusCode, string.Empty);
            }

            // sum quantity for same part no
            inventoryRequest.Parts = _inventoryService.SumQuantityForSamePartNo(inventoryRequest.Parts);

            var outboxEvent = _outboxEventService.CreateOutboxEventPerOrder(request, Enum.Parse<Domain.Utilities.InputTransactionType>(request.EventType), request.ReceiveOrderNo);
            await _inventoryService.AdjustPartsQuantityAsync(inventoryRequest, Enum.Parse<Domain.Utilities.InputTransactionType>(request.EventType), outboxEvent);
            applicationResponse = ApplicationResponseBuilder.Ok();
            baseResponse = _mapper.Map(applicationResponse);
            return StatusCode((int)applicationResponse.HttpStatusCode, baseResponse);
        }

        [Authorize(AuthenticationSchemes = AuthenConstant.DefaultScheme)]
        [HttpPost, Route("request/event-handler")]
        [ValidateModel]
        public async Task<IActionResult> RequestEventHandler([FromBody] Domain.DTO.Requests.RequestEvent request)
        {
            ApplicationResponse applicationResponse;
            Domain.DTO.Responses.BaseResponse baseResponse;

            var eventId = Request.Headers.GetValueHeader(EventConstant.EVENT_ID);
            var eventType = Request.Headers.GetValueHeader(EventConstant.EVENT_TYPE);

            // validate event id and event type must be values
            if (string.IsNullOrEmpty(eventId) || string.IsNullOrEmpty(eventType))
            {
                _logger.LogWarning("The event id or event type isn't value.");
                applicationResponse = ApplicationResponseBuilder.BadRequest();
                baseResponse = _mapper.Map(applicationResponse);
                return StatusCode((int)applicationResponse.HttpStatusCode, baseResponse);
            }

            // check idempotent
            if (_unitOfWork.IdempotentRepository.Find(x => x.EventId == eventId && x.EventType == eventType).Any())
            {
                _logger.LogInformation($"These even id: \"{eventId}\" and event type: \"{eventType}\" has been in the system.");
                applicationResponse = ApplicationResponseBuilder.Ok();
                baseResponse = _mapper.Map(applicationResponse);
                return StatusCode((int)applicationResponse.HttpStatusCode, baseResponse);
            }

            // Check warehouse location no and partno on master
            var inventoryRequest = _eventRequestMapper.Map(request);
            bool isFound = await _inventoryService.ValidateModelListFromMaster(inventoryRequest, Enum.Parse<Domain.Utilities.InputTransactionType>(request.EventType));
            if (!isFound)
            {
                applicationResponse = ApplicationResponseBuilder.NotFound();
                return StatusCode((int)applicationResponse.HttpStatusCode, string.Empty);
            }

            // sum quantity for same part no
            inventoryRequest.Parts = _inventoryService.SumQuantityForSamePartNo(inventoryRequest.Parts);

            var outboxEvent = _outboxEventService.CreateOutboxEventPerOrder(request, Enum.Parse<Domain.Utilities.InputTransactionType>(request.EventType), request.RequestOrderNo);
            await _inventoryService.AdjustPartsQuantityAsync(inventoryRequest, Enum.Parse<Domain.Utilities.InputTransactionType>(request.EventType), outboxEvent);
            applicationResponse = ApplicationResponseBuilder.Ok();
            baseResponse = _mapper.Map(applicationResponse);
            return StatusCode((int)applicationResponse.HttpStatusCode, baseResponse);
        }
    }
}
