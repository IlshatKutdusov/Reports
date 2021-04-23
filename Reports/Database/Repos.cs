using Microsoft.EntityFrameworkCore;
using Reports.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Reports.API.Database
{
    public class Repos : IRepos
    {
        private readonly DataContext _context;

        public Repos(DataContext context)
        {
            _context = context;
        }

        public IQueryable<T> Get<T>() where T : class, IBaseEntity
        {
            return _context.Set<T>().Where(x => x.isActive).AsQueryable();
        }

        public IQueryable<T> Get<T>(Expression<Func<T, bool>> selector) where T : class, IBaseEntity
        {
            return _context.Set<T>().Where(selector).Where(x => x.isActive).AsQueryable();
        }

        public async Task<int> Add<T>(T newEntity) where T : class, IBaseEntity
        {
            var entity = await _context.Set<T>().AddAsync(newEntity);
            return entity.Entity.Id;
        }

        public async Task AddRange<T>(IEnumerable<T> newEntities) where T : class, IBaseEntity
        {
            await _context.Set<T>().AddRangeAsync(newEntities);
        }

        public async Task Delete<T>(int id) where T : class, IBaseEntity
        {
            var activeEntity = await _context.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
            activeEntity.isActive = false;
            await Task.Run(() => _context.Update(activeEntity));
        }

        public async Task Remove<T>(T entity) where T : class, IBaseEntity
        {
            await Task.Run(() => _context.Set<T>().Remove(entity));
        }

        public async Task RemoveRange<T>(IEnumerable<T> entities) where T : class, IBaseEntity
        {
            await Task.Run(() => _context.Set<T>().RemoveRange(entities));
        }

        public async Task Update<T>(T entity) where T : class, IBaseEntity
        {
            await Task.Run(() => _context.Set<T>().Update(entity));
        }

        public async Task UpdateRange<T>(IEnumerable<T> entities) where T : class, IBaseEntity
        {
            await Task.Run(() => _context.Set<T>().UpdateRange(entities));
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public IQueryable<T> GetAll<T>() where T : class, IBaseEntity
        {
            return _context.Set<T>().AsQueryable();
        }
    }
}
