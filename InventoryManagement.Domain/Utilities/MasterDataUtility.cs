using InventoryManagement.Domain.DTO.Models;
using System.Collections.Generic;
using System.Linq;

namespace InventoryManagement.Domain.Utilities
{
    public static class MasterDataUtility
    {
        public static IEnumerable<PartModel> CombinePartsWithMasterData(Client.Models.MasterClient.PartModel marterDataPart, IEnumerable<string> partNoList)
        {
            List<PartModel> parts = new List<PartModel>();
            if (marterDataPart != null)
            {
                foreach (var partNo in partNoList)
                {
                    var part = new PartModel();
                    part.PartNo = partNo;
                    part.PartName = marterDataPart?.Data?.FirstOrDefault(x => x.No == partNo)?.Name;
                    parts.Add(part);
                }
            }
            return parts;
        }

        public static IEnumerable<DTO.Models.WarehouseLocationModel> CombineWarehouseWithMasterData(
            Client.Models.MasterClient.WarehouseLocationModel masterWarehouses
            , IEnumerable<string> warehouses)
        {
            var rows = new List<DTO.Models.WarehouseLocationModel>();
            foreach (var warehouse in warehouses)
            {
                var result = new DTO.Models.WarehouseLocationModel();
                var master = masterWarehouses?.Data?.FirstOrDefault(x => x.Code == warehouse);
                result.Code = warehouse;
                result.Description = master?.Name;
                result.Address = master?.Description;
                rows.Add(result);
            }

            return rows;
        }
    }
}
