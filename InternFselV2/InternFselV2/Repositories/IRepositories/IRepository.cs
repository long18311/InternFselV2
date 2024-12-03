using InternFselV2.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace InternFselV2.Repositories.IRepositories
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        IQueryable<TEntity> Queryable { get; }
        public Task<TEntity?> GetbyId(Guid id);
        public Task<IList<TEntity>?> GetbyIds(IEnumerable<Guid> ids);
        public Task<TEntity?> Create(TEntity entity);
        public Task<TEntity?> Update(TEntity entity);
        public Task<bool> Delete(TEntity entity);
        public Task<bool> DeleteById(Guid id);

    }
}
