using Boxed.Mapping;
using InventoryManagement.Domain.Client;
using InventoryManagement.Domain.DTO.Models;
using InventoryManagement.Domain.Utilities;
using InventoryManagement.Infrastructure;
using InventoryManagement.Infrastructure.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace InventoryManagement.Domain.Services
{
    public class InventoryService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper<Inventory, DTO.Responses.Row> _inventoryMapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMasterClient _masterClient;
        private readonly IMapper<DTO.Requests.Part, DTO.Requests.Inventory> _requestInventoryPartMapper;
        private readonly OutboxEventService _outboxService;

        public InventoryService(UnitOfWork unitOfWork
            , IMapper<Inventory, DTO.Responses.Row> inventoryMapper
            , IHttpContextAccessor httpContextAccessor
            , IMasterClient masterClient
            , IMapper<DTO.Requests.Part, DTO.Requests.Inventory> requestInventoryPartMapper
            , OutboxEventService outboxService)
        {
            _unitOfWork = unitOfWork;
            _inventoryMapper = inventoryMapper;
            _httpContextAccessor = httpContextAccessor;
            _masterClient = masterClient;
            _requestInventoryPartMapper = requestInventoryPartMapper;
            _outboxService = outboxService;
        }

        public async Task AdjustPartQuantityAsync(DTO.Requests.Inventory request)
        {
            string userId = _httpContextAccessor.HttpContext.GetUserId();
            string userName = _httpContextAccessor.HttpContext.GetUserName();
            
            Inventory currentInventory = _unitOfWork.InventoryRepository.Find(x => x.PartNo == request.PartNo && x.WarehouseLocationNo == request.WarehouseLocationNo, null).FirstOrDefault();

            var outboxEvent = _outboxService.CreateOutboxEventPerPart(request, currentInventory);

            // If available quantity not enough
            if (outboxEvent.Type == nameof(InventoryTransactionType.STOCK_BOOKING_FAILED))
            {
                await _unitOfWork.OutboxEventRepository.AddAsync(outboxEvent);
            }
            else
            {
                Inventory newInventory = CalculateInventory(request, currentInventory);
                using (var transactionDatabase = _unitOfWork.BeginTransaction())
                {
                    try
                    {
                        await _unitOfWork.OutboxEventRepository.AddAsync(outboxEvent);
                        await DecideInventoryAddOrUpdate(currentInventory, newInventory, userId, userName);

                        await transactionDatabase.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transactionDatabase.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public async Task AdjustPartsQuantityAsync(DTO.Requests.InventoryRequest request, InputTransactionType inputType, OutboxEvent outboxEvent)
        {
            string userId = _httpContextAccessor.HttpContext.GetUserId();
            string userName = _httpContextAccessor.HttpContext.GetUserName();
            var requestContext = _httpContextAccessor.HttpContext.Request;

            var requestInventoryList = GetRequestInventoryFromRequestParts(request.Parts, inputType, request.WarehouseLocationNo);
            var partNoList = request.Parts.Select(x => x.No).ToList();
            var currentInventoryList = await _unitOfWork.InventoryRepository.Find(x => x.WarehouseLocationNo == request.WarehouseLocationNo && partNoList.Contains(x.PartNo)).ToListAsync();
            outboxEvent = _outboxService.CheckAvailableQuantity(outboxEvent, request, currentInventoryList);
            
            // If available quantity not enough
            if (outboxEvent.Type == nameof(InventoryTransactionType.STOCK_BOOKING_FAILED))
            {
                
                using(var transactionDatabase = _unitOfWork.BeginTransaction())
                {
                    try
                    {
                        await _unitOfWork.OutboxEventRepository.AddAsync(outboxEvent);

                        // Add Idempotent
                        await DecideForAddIdempotent(requestContext.Headers.GetValueHeader(EventConstant.EVENT_ID), requestContext.Headers.GetValueHeader(EventConstant.EVENT_TYPE));

                        await transactionDatabase.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transactionDatabase.RollbackAsync();
                        throw;
                    }
                }
            }
            else
            {
                using (var transactionDatabase = _unitOfWork.BeginTransaction())
                {
                    try
                    {
                        await _unitOfWork.OutboxEventRepository.AddAsync(outboxEvent);
                        foreach (var requestInventory in requestInventoryList)
                        {
                            Inventory currentInventory = currentInventoryList.Where(x => x.PartNo == requestInventory.PartNo && x.WarehouseLocationNo == request.WarehouseLocationNo).FirstOrDefault();
                            Inventory newInventory = CalculateInventory(requestInventory, currentInventory);
                            await DecideInventoryAddOrUpdate(currentInventory, newInventory, userId, userName);
                        }

                        // Add Idempotent
                        await DecideForAddIdempotent(requestContext.Headers.GetValueHeader(EventConstant.EVENT_ID), requestContext.Headers.GetValueHeader(EventConstant.EVENT_TYPE));

                        await transactionDatabase.CommitAsync();

                    }
                    catch (Exception)
                    {
                        await transactionDatabase.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        internal IEnumerable<DTO.Requests.Inventory> GetRequestInventoryFromRequestParts(IEnumerable<DTO.Requests.Part> parts, InputTransactionType inputType, string warehouseLocationNo)
        {
            var requestPartList = new List<DTO.Requests.Inventory>();
            foreach(var part in parts)
            {
                var requestPart = _requestInventoryPartMapper.Map(part);
                requestPart.TransactionType = inputType.ToString();
                requestPart.WarehouseLocationNo = warehouseLocationNo;
                requestPartList.Add(requestPart);
            }
            return requestPartList;
        }

        public IEnumerable<DTO.Requests.Part> SumQuantityForSamePartNo(IEnumerable<DTO.Requests.Part> request)
        {
            return request.GroupBy(g => g.No).Select(x => new DTO.Requests.Part { No = x.Key, Qty = x.Sum(y => y.Qty), Uom = x.First().Uom });
        }

        private async Task DecideInventoryAddOrUpdate(Inventory currentInventory, Inventory newInventory, string userId, string userName)
        {
            if (currentInventory == null)
            {
                await _unitOfWork.InventoryRepository.AddAsync(newInventory, userId, userName);
            }
            else
            {
                await _unitOfWork.InventoryRepository.UpdateAsync(newInventory, userId, userName);
            }
        }

        internal async Task DecideForAddIdempotent(string eventId, string eventType)
        {
            if(!string.IsNullOrEmpty(eventId) && !string.IsNullOrEmpty(eventType))
            {
                await _unitOfWork.IdempotentRepository.AddAsync(
                    new Idempotent { EventId = eventId, EventType = eventType }
                );
            }
        }

        internal Inventory CalculateInventory(DTO.Requests.Inventory request, Inventory inventory)
        {
            if(inventory == null)
            {
                inventory = new Inventory { 
                    PartNo = request.PartNo,
                    WarehouseLocationNo = request.WarehouseLocationNo,
                    Uom = request.Uom
                };
            }

            switch(Enum.Parse<InputTransactionType>(request.TransactionType))
            {
                case InputTransactionType.RC_SUBMITTED:
                    inventory.POQty += request.Quantity;
                    break;
                case InputTransactionType.RC_ACCEPTED:
                    inventory.ReceivingQty += request.Quantity;
                    inventory.POQty -= request.Quantity;
                    break;
                case InputTransactionType.RC_COMPLETED:
                    inventory.BalanceQty += request.Quantity;
                    inventory.ReceivingQty -= request.Quantity;
                    break;
                case InputTransactionType.RQ_SUBMITTED:
                    inventory.RequestQty += request.Quantity;
                    break;
                case InputTransactionType.RQ_DELIVERED:
                    inventory.BalanceQty -= request.Quantity;
                    inventory.RequestQty -= request.Quantity;
                    break;
            }

            // Calulate Avaliable Quantity
            inventory.AvailableQty = inventory.BalanceQty - inventory.RequestQty;

            return inventory;
        }

        public async Task<DTO.Responses.Inventory> GetStockInventoryAsync(SieveModel sieveModel, string warehouseLocation, string partNo)
        {
            var response = new DTO.Responses.Inventory();
            var masterDataParts = await _masterClient.GetPartsAsync();
            IEnumerable<PartModel> partMapMasterList;

            // Has warehouseLocation and partNo
            if (!string.IsNullOrEmpty(warehouseLocation) && !string.IsNullOrEmpty(partNo))
            {
                var result = await _unitOfWork.InventoryRepository.Find(x => x.WarehouseLocationNo == warehouseLocation && x.PartNo == partNo, sieveModel).ToListAsync();
                partMapMasterList = MasterDataUtility.CombinePartsWithMasterData(masterDataParts, result.Select(x => x.PartNo));
                response.Result.WarehouseLocationNo = warehouseLocation;
                response.Result.Rows = _inventoryMapper.MapList(result);
            }
            else
            {
                // Has warehouseLocation only
                if(!string.IsNullOrEmpty(warehouseLocation))
                {
                    var result = await _unitOfWork.InventoryRepository.Find(x => x.WarehouseLocationNo == warehouseLocation, sieveModel).ToListAsync();
                    partMapMasterList = MasterDataUtility.CombinePartsWithMasterData(masterDataParts, result.Select(x => x.PartNo));
                    response.Result.WarehouseLocationNo = warehouseLocation;
                    response.Result.Rows = _inventoryMapper.MapList(result);
                }

                // Has partNo only
                else if(!string.IsNullOrEmpty(partNo))
                {
                    var result = await _unitOfWork.InventoryRepository.Find(x => x.PartNo == partNo, sieveModel).ToListAsync();
                    result = SumAllQuantitiesForSamePartNo(result).ToList();
                    partMapMasterList = MasterDataUtility.CombinePartsWithMasterData(masterDataParts, result.Select(x => x.PartNo));
                    response.Result.WarehouseLocationNo = InventoryConstant.ALL;
                    response.Result.Rows = _inventoryMapper.MapList(result);
                }

                // Don't have values
                else
                {
                    var result  = await _unitOfWork.InventoryRepository.FindAll(sieveModel).ToListAsync();
                    result = SumAllQuantitiesForSamePartNo(result).ToList();
                    partMapMasterList = MasterDataUtility.CombinePartsWithMasterData(masterDataParts, result.Select(x => x.PartNo));
                    response.Result.WarehouseLocationNo = InventoryConstant.ALL;
                    response.Result.Rows = _inventoryMapper.MapList(result);
                }
            }

            // Set part name from master data
            if (partMapMasterList != null)
            {
                foreach (var row in response.Result.Rows)
                {
                    row.PartName = partMapMasterList?.FirstOrDefault(x => x.PartNo == row.PartNo)?.PartName;
                }
            }

            return response;
        }

        internal IEnumerable<Inventory> SumAllQuantitiesForSamePartNo(IEnumerable<Inventory> inventories)
        {
            return inventories.GroupBy(x => x.PartNo).Select(x => new Inventory
            {
                PartNo = x.Key,
                POQty = x.Sum(s => s.POQty),
                ReceivingQty = x.Sum(s => s.ReceivingQty),
                BalanceQty = x.Sum(s => s.BalanceQty),
                RequestQty = x.Sum(s => s.RequestQty),
                AvailableQty = x.Sum(s => s.AvailableQty),
                Uom = x.FirstOrDefault()?.Uom
            });
        }
        
        public async Task<bool> ValidateModelFromMaster(DTO.Requests.Inventory request)
        {
            var partNo = await _masterClient.GetPartsAsync();
            var warehouseNo = await _masterClient.GetWarehouseNoAsync();

            // Cann't fetch data from master service
            if(partNo == null || partNo?.Data == null ||
               warehouseNo == null || warehouseNo?.Data == null)
            {
                return false;
            }

            var part = partNo.Data.FirstOrDefault(x => x.No == request.PartNo);
            // Check partNo from masterdata
            if (part == null)
            {
                return false;
            }

            // Check uom of part from masterdata
            if (part.UomId == null || part.UomId.Name != request.Uom)
            {
                return false;
            }

            // Check warehouseNo from masterdata
            if (!warehouseNo.Data.Any(x => x.Code == request.WarehouseLocationNo))
            {
                return false;
            }

            return true;
        }

        public async Task<bool> ValidateModelListFromMaster(DTO.Requests.InventoryRequest request, InputTransactionType inputType)
        {
            var requestPartList = GetRequestInventoryFromRequestParts(request.Parts, inputType, request.WarehouseLocationNo);
            foreach (var inventoryRequest in requestPartList)
            {
                var isFound = await ValidateModelFromMaster(inventoryRequest);
                if (!isFound)
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<IList<AvailableOfPathModel>> ValidateQuantity(DTO.Requests.InventoryRequest request)
        {
            List<AvailableOfPathModel> availableQtyParts = new List<AvailableOfPathModel>();

            foreach(var part in request.Parts)
            {
                var availableQty = await _unitOfWork.InventoryRepository.Find(x => x.WarehouseLocationNo == request.WarehouseLocationNo && x.PartNo == part.No)
                    .Select(x => new AvailableOfPathModel { PartNo = x.PartNo, AvailableQuantity = x.AvailableQty, Qty = part.Qty, Uom = part.Uom })
                    .SingleOrDefaultAsync();

                if(availableQty == null)
                {
                    availableQty = new AvailableOfPathModel { PartNo = part.No, AvailableQuantity = 0, Qty = part.Qty, Uom = part.Uom };
                }

                if(part.Qty > availableQty.AvailableQuantity)
                {
                    availableQty.IsEnough = false;
                }
                else
                {
                    availableQty.IsEnough = true;
                }
                availableQtyParts.Add(availableQty);
            }
            return availableQtyParts;
        }
    }
}
