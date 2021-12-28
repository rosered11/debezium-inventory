using InventoryManagement.Domain.Services;
using InventoryManagement.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace InventoryManagement.Tests.Services.InventoryServiceTest
{
    public class DecideForAddIdempotentTest
    {
        private static InventoryService CreateInventoryService(UnitOfWork unit)
            => new InventoryService(unit, null, null, null, null, null);

        [Theory]
        [InlineData(null, "")]
        [InlineData(null, "a")]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("", null)]
        [InlineData("b", null)]
        public async Task WhenEventIdAndEventTypeDontHaveSomeValue_ShouldNotSaveIdempotent(string eventId, string eventType)
        {
            using (var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
            ))
            {
                var service = CreateInventoryService(new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor())));

                await service.DecideForAddIdempotent(eventId, eventType);

                Assert.Empty(context.Idempotent);
            }
        }

        [Fact]
        public async Task WhenEventIdAndEventTypeHaveValue_ShouldSaveIdempotent()
        {
            using (var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}").Options
            ))
            {
                var service = CreateInventoryService(new UnitOfWork(context, new SieveProcessor(new SieveOptionsAccessor())));

                await service.DecideForAddIdempotent("eventId", "eventType");

                Assert.Single(context.Idempotent);
                var result = await context.Idempotent.FirstAsync();
                Assert.Equal("eventId", result.EventId);
                Assert.Equal("eventType", result.EventType);
            }
        }
    }
}
