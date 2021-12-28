using InventoryManagement.Infrastructure.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Sieve.Models;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace InventoryManagement.Infrastructure.Repositories
{
    public class Repository<TEntity> where TEntity : class
    {
        private readonly ISieveProcessor _processor;
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public Repository(ApplicationDbContext context, ISieveProcessor processor)
        {
            _processor = processor;
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> Get(SieveModel query)
        {
            var result = _processor.Apply(query, _dbSet);

            return result.AsEnumerable();
        }

        public virtual IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, SieveModel query)
        {
            var result = _processor.Apply(query, _dbSet);
            return result.Where(predicate).AsNoTracking();
        }

        public virtual IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate).AsNoTracking();
        }

        public virtual IQueryable<TEntity> FindAll(SieveModel query)
        {
            var result = _processor.Apply(query, _dbSet);
            return result.AsNoTracking();
        }

        public virtual IQueryable<TEntity> FindAll()
        {
            return _dbSet.AsNoTracking();
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task AddAsync(TEntity entity, string userId, string userName)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync(userId, userName);
        }

        public virtual async Task UpdateAsync(TEntity entity, string userId, string userName)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync(userId, userName);
        }
    }
}
