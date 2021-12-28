using InventoryManagement.Infrastructure.Entities;
using InventoryManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Sieve.Services;
using System;

namespace InventoryManagement.Infrastructure
{
    public class UnitOfWork : IDisposable
    {
        private readonly ApplicationDbContext _context;
        public Repository<Inventory> InventoryRepository { get; }
        public Repository<OutboxEvent> OutboxEventRepository { get; }
        public Repository<Idempotent> IdempotentRepository { get; }
        public UnitOfWork(ApplicationDbContext context, ISieveProcessor processor)
        {
            _context = context;
            InventoryRepository = InventoryRepository ?? new Repository<Inventory>(context, processor);
            OutboxEventRepository = OutboxEventRepository ?? new Repository<OutboxEvent>(context, processor);
            IdempotentRepository = IdempotentRepository ?? new Repository<Idempotent>(context, processor);
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
