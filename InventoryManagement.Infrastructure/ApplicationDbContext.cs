using InventoryManagement.Infrastructure.Entities;
using InventoryManagement.Infrastructure.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryManagement.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Set composit keys
            modelBuilder.Entity<Inventory>()
                .HasKey(b => new { b.PartNo, b.WarehouseLocationNo })
                .HasName("PrimaryKey_PartNoAndWarehouseLocationNo");

            // Set concurrency token
            modelBuilder.Entity<Inventory>().UseXminAsConcurrencyToken();

            // Set Idempotent keys
            modelBuilder.Entity<Idempotent>()
                .HasKey(b => new { b.EventId, b.EventType })
                .HasName("PrimaryKey_EventIdAndEventType");

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<OutboxEvent> OutboxEvent { get; set; }
        public DbSet<Idempotent> Idempotent { get; set; }

        public Task<int> SaveChangesAsync(string userId, string userName, CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.Now;
            BeforeSaveChangesForBaseEntity(userId, userName, now);
            return SaveChangesAsync(now, cancellationToken);
        }

        private Task<int> SaveChangesAsync(DateTimeOffset now, CancellationToken cancellationToken = default)
        {
            BeforeSaveChangesForOutboxEvent(now);
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            BeforeSaveChangesForOutboxEvent(DateTimeOffset.Now);
            return base.SaveChangesAsync(cancellationToken);
        }

        private void BeforeSaveChangesForBaseEntity(string userId, string userName, DateTimeOffset now)
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((BaseEntity)entity.Entity).CreatedAt = now;
                    ((BaseEntity)entity.Entity).UserIdCreated = userId;
                    ((BaseEntity)entity.Entity).UserCreated = userName;
                }
                ((BaseEntity)entity.Entity).UpdatedAt = now;
                ((BaseEntity)entity.Entity).UserIdUpdated = userId;
                ((BaseEntity)entity.Entity).UserUpdated = userName;
            }
        }
    
        private void BeforeSaveChangesForOutboxEvent(DateTimeOffset now)
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is OutboxEvent && (x.State == EntityState.Added));

            foreach (var entity in entities)
            {
                ((OutboxEvent)entity.Entity).TimeStamp = now;
            }
        }
    }
}
