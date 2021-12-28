using InventoryManagement.Infrastructure;
using InventoryManagement.Infrastructure.Entities;
using InventoryManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;
using System;
using System.Linq;
using Xunit;

namespace InventoryManagement.Tests.Repositories.RepositoryTest
{
    public class FindTest
    {
        [Fact]
        public void WhenFindInventory_AndConditionMatch_ShouldReturnInventory()
        {
            using(var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
            ))
            {
                context.Inventories.Add(new Inventory { 
                    PartNo = "partno1",
                    WarehouseLocationNo = "warehouse1"
                });
                context.SaveChanges();
                var repository = new Repository<Inventory>(context, new SieveProcessor(new SieveOptionsAccessor()));
                
                var result = repository.Find(x => x.PartNo == "partno1" && x.WarehouseLocationNo == "warehouse1", new Sieve.Models.SieveModel()).FirstOrDefault();

                Assert.NotNull(result);
                Assert.Equal("partno1", result.PartNo);
                Assert.Equal("warehouse1", result.WarehouseLocationNo);
            }
        }

        [Fact]
        public void WhenFindInventory_AndConditionNotMatch_ShouldReturnNull()
        {
            using (var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
            ))
            {
                context.Inventories.Add(new Inventory
                {
                    PartNo = "partno1",
                    WarehouseLocationNo = "warehouse1"
                });
                context.SaveChanges();
                var repository = new Repository<Inventory>(context, new SieveProcessor(new SieveOptionsAccessor()));

                var result = repository.Find(x => x.PartNo == "partno2" && x.WarehouseLocationNo == "warehouse2", new Sieve.Models.SieveModel()).FirstOrDefault();

                Assert.Null(result);
            }
        }
    }
}
