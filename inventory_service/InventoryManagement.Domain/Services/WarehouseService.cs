using InventoryManagement.Domain.Client;
using InventoryManagement.Domain.Utilities;
using InventoryManagement.Infrastructure;
using Sieve.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagement.Domain.Services
{
    public class WarehouseService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMasterClient _masterClient;

        public WarehouseService(UnitOfWork unitOfWork, IMasterClient masterClient)
        {
            _unitOfWork = unitOfWork;
            _masterClient = masterClient;
        }

        public async Task<DTO.Responses.WarehouseLocation> GetWarehouseLocationHasBalanceAsync(SieveModel sieveModel)
        {
            var inventoryHasStocks = _unitOfWork.InventoryRepository.Find(x => x.BalanceQty > 0, sieveModel);
            var warehouses = inventoryHasStocks.GroupBy(x => x.WarehouseLocationNo).Select(x => x.Key).ToList();
            var masterWarehouse = await _masterClient.GetWarehouseNoAsync();

            var warehosesResult = MasterDataUtility.CombineWarehouseWithMasterData(masterWarehouse, warehouses.Select(x => x));
            var response = new DTO.Responses.WarehouseLocation();

            var rows = new List<DTO.Responses.WarehouseLocation.Row>();
            foreach (var warehouse in warehouses)
            {
                var result = new DTO.Responses.WarehouseLocation.Row();
                var master = warehosesResult.FirstOrDefault(x => x.Code == warehouse);
                result.Code = warehouse;
                result.Description = master?.Description;
                result.Address = master?.Address;
                rows.Add(result);
            }

            response.Result.Rows = rows;
            return response;
        }
    }
}
