using InternFselV2.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace InternFselV2.Model.Interfaces
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        IQueryable<TEntity> Queryable { get; }
        IUnitOfWork UnitOfWork { get; }
        public Task<TEntity?> GetbyId(Guid id);
        public Task<TEntity?> GetById(Guid id);
        public Task<TEntity?> GetById(Guid id, params Expression<Func<TEntity, object>>[] includes);
        public Task<IList<TEntity>?> GetbyIds(IEnumerable<Guid> ids);
        public TEntity? Create(TEntity entity);
        public TEntity? Update(TEntity entity);
        public bool Delete(TEntity entity);
        public Task<TEntity?> CreateAsync(TEntity entity);
        public Task<TEntity?> UpdateAsync(TEntity entity);
        public Task<bool> DeleteAsync(TEntity entity);
        public string GetSQLCreateEntity<T>(T entity) where T : Entity;

    }
}
