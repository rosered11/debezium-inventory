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
    public class FindAllTest
    {
        [Fact]
        public void WhenFindAllInventory_ShouldReturnAllInventory()
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
                context.Inventories.Add(new Inventory
                {
                    PartNo = "partno2",
                    WarehouseLocationNo = "warehouse2"
                });
                context.SaveChanges();
                var repository = new Repository<Inventory>(context, new SieveProcessor(new SieveOptionsAccessor()));

                var result = repository.FindAll(new Sieve.Models.SieveModel()).ToList();

                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
            }
        }

        [Fact]
        public void WhenFindAllInventory_ButWithoutInventory_ShouldReturnEmpty()
        {
            using (var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
            ))
            {
                var repository = new Repository<Inventory>(context, new SieveProcessor(new SieveOptionsAccessor()));

                var result = repository.FindAll(new Sieve.Models.SieveModel()).ToList();

                Assert.NotNull(result);
                Assert.Empty(result);
            }
        }
    }
}
