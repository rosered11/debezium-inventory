using InventoryManagement.Domain.Client.Models.MasterClient;
using InventoryManagement.Domain.Utilities;
using System.Collections.Generic;
using Xunit;

namespace InventoryManagement.Tests.Services
{
    public class MasterDataUtilityTest
    {
        #region CombineWarehouseWithMasterData
        [Fact]
        public void WhenPassWarehousesInInventoryAndWarehouseMasterData_ShouldCombineIsCorrect()
        {
            var warehouses = new List<string> { "w1" };
            var warehousesMaster = new WarehouseLocationModel
            {
                Data = new List<WarehouseLocationModel.DataModel> {
                new WarehouseLocationModel.DataModel
                {
                    Name = "wname",
                    Description = "wdesc",
                    Code = "w1"
                }
            }
            };

            var result = MasterDataUtility.CombineWarehouseWithMasterData(warehousesMaster, warehouses);

            Assert.Collection(result,
                x => {
                    Assert.Equal("wname", x.Description);
                    Assert.Equal("w1", x.Code);
                    Assert.Equal("wdesc", x.Address);
                }
            );
        }

        [Fact]
        public void WhenPassWarehousesInInventory_ButMasterWarehousesAreNull_ShouldCombineIsCorrect()
        {
            var warehouses = new List<string> { "w1" };
            WarehouseLocationModel warehousesMaster = null;

            var result = MasterDataUtility.CombineWarehouseWithMasterData(warehousesMaster, warehouses);

            Assert.Collection(result,
                x => {
                    Assert.Null(x.Description);
                    Assert.Equal("w1", x.Code);
                    Assert.Null(x.Address);
                }
            );
        }

        [Fact]
        public void WhenPassWarehousesInInventory_ButMasterWarehousesAreEmpty_ShouldCombineIsCorrect()
        {
            var warehouses = new List<string> { "w1" };
            WarehouseLocationModel warehousesMaster = new WarehouseLocationModel();

            var result = MasterDataUtility.CombineWarehouseWithMasterData(warehousesMaster, warehouses);

            Assert.Collection(result,
                x => {
                    Assert.Null(x.Description);
                    Assert.Equal("w1", x.Code);
                    Assert.Null(x.Address);
                }
            );
        }
        #endregion
    }
}
