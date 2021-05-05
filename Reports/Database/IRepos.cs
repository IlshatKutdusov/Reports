﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Reports.Entities;
using Reports.Models;

namespace Reports.Database
{
    public interface IRepos
    {
        IQueryable<T> Get<T>() where T : class, IBaseEntity;
        IQueryable<T> GetAll<T>() where T : class, IBaseEntity;

        Task Add<T>(T newEntity) where T : class, IBaseEntity;
        Task AddRange<T>(IEnumerable<T> newEntities) where T : class, IBaseEntity;

        Task Delete<T>(int entity) where T : class, IBaseEntity;

        Task Remove<T>(int entity) where T : class, IBaseEntity;
        Task Remove<T>(T entity) where T : class, IBaseEntity;
        Task RemoveRange<T>(IEnumerable<T> entities) where T : class, IBaseEntity;

        Task Update<T>(T entity) where T : class, IBaseEntity;

        Task SaveChangesAsync();
    }
}
