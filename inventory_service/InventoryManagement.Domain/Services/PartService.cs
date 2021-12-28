using InventoryManagement.Domain.Client;
using InventoryManagement.Domain.DTO.Models;
using InventoryManagement.Domain.DTO.Responses;
using InventoryManagement.Domain.Utilities;
using InventoryManagement.Infrastructure;
using Sieve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace InventoryManagement.Domain.Services
{
    public class PartService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMasterClient _masterClient;

        public PartService(UnitOfWork unitOfWork, IMasterClient masterClient)
        {
            _unitOfWork = unitOfWork;
            _masterClient = masterClient;
        }
        public async Task<PartOnly> GetPartsThatHaveAvailableQtyInStock(SieveModel sieveModel)
        {
            var partsHaveAvailable = _unitOfWork.InventoryRepository.Find(x => x.AvailableQty > 0, sieveModel);

            var response = new PartOnly();
            response.Result.Rows = await MappingPartsAsync(partsHaveAvailable);
            return response;
        }

        public async Task<PartOnly> GetPartsSpecificTypeQtyInStock(SieveModel sieveModel, QuantityTypeEnum quantityType)
        {
            var expression = GetConditionPartFollowQuantityType(quantityType);
            var parts = _unitOfWork.InventoryRepository.Find(expression, sieveModel);

            var response = new PartOnly();
            response.Result.Rows = await MappingPartsAsync(parts);
            return response;
        }

        internal Expression<Func<Infrastructure.Entities.Inventory, bool>> GetConditionPartFollowQuantityType(QuantityTypeEnum quantityType)
        {
            switch(quantityType)
            {
                case QuantityTypeEnum.AVAILABLE:
                    return x => x.AvailableQty > 0;
                case QuantityTypeEnum.BALANCE:
                    return x => x.BalanceQty > 0;
            }
            throw new ArgumentException("Can't get condition follow this quantity type");
        }

        public async Task<PartAndWarehouseLocation> GetPartsThatHaveAvailableQtyInStock(SieveModel sieveModel, string warehouseLocationNo)
        {
            var partsHaveAvailable = _unitOfWork.InventoryRepository.Find(x => x.AvailableQty > 0 && x.WarehouseLocationNo == warehouseLocationNo, sieveModel);

            var response = new PartAndWarehouseLocation();
            response.Result.WarehouseCode = warehouseLocationNo;
            response.Result.Rows = await MappingPartsAsync(partsHaveAvailable);
            var warehouseMaster = await GetWarehouseAsync(warehouseLocationNo);
            response.Result.WarehouseDescription = warehouseMaster?.Description;

            return response;
        }

        public async Task<PartAndWarehouseLocation> GetPartsSpecificTypeQtyInStock(SieveModel sieveModel, string warehouseLocationNo, QuantityTypeEnum quantityType)
        {
            var expression = GetConditionPartFollowQuantityType(quantityType);
            var parts = _unitOfWork.InventoryRepository.Find(x => x.WarehouseLocationNo == warehouseLocationNo, sieveModel).Where(expression);

            var response = new PartAndWarehouseLocation();
            response.Result.WarehouseCode = warehouseLocationNo;
            response.Result.Rows = await MappingPartsAsync(parts);
            var warehouseMaster = await GetWarehouseAsync(warehouseLocationNo);
            response.Result.WarehouseDescription = warehouseMaster?.Description;

            return response;
        }

        internal async Task<WarehouseLocationModel> GetWarehouseAsync(string warehouseLocationNo)
        {
            var masterWarehouse = await _masterClient.GetWarehouseNoAsync();
            var warehoseMappingMasterList = MasterDataUtility.CombineWarehouseWithMasterData(masterWarehouse, new[] { warehouseLocationNo });
            return warehoseMappingMasterList.FirstOrDefault(x => x.Code == warehouseLocationNo);
        }

        internal async Task<IEnumerable<PartBase.Row>> MappingPartsAsync(IQueryable<Infrastructure.Entities.Inventory> partsHaveAvailable)
        {
            var partNoThatHaveAvailable = partsHaveAvailable.GroupBy(x => x.PartNo).Select(x => new PartNoWithAvailable { PartNo = x.Key, Available = x.Sum(s => s.AvailableQty) }).ToList();
            var masterDataParts = await _masterClient.GetPartsAsync();
            var partMappingMasterList = MasterDataUtility.CombinePartsWithMasterData(masterDataParts, partNoThatHaveAvailable.Select(x => x.PartNo));

            List<PartBase.Row> rows = new List<PartBase.Row>();

            foreach (var part in partNoThatHaveAvailable)
            {
                var row = new PartBase.Row();
                row.PartNo = part.PartNo;
                row.Available = part.Available;
                row.Uom = partsHaveAvailable.FirstOrDefault(x => x.PartNo == part.PartNo).Uom;
                row.PartName = partMappingMasterList.FirstOrDefault(x => x.PartNo == part.PartNo)?.PartName;
                rows.Add(row);
            }
            return rows;
        }
    }
}
