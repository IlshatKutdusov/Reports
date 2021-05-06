using Microsoft.EntityFrameworkCore;
using Reports.Entities;
using Reports.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Reports.Database
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

        public IQueryable<T> GetAll<T>() where T : class, IBaseEntity
        {
            return _context.Set<T>().AsQueryable();
        }

        public async Task Add<T>(T newEntity) where T : class, IBaseEntity
        {
            await _context.Set<T>().AddAsync(newEntity);
        }

        public async Task AddRange<T>(IEnumerable<T> newEntities) where T : class, IBaseEntity
        {
            await _context.Set<T>().AddRangeAsync(newEntities);
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

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}