using InternFselV2.Entities;
using InternFselV2.Model.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace InternFselV2.Repositories.Repositories
{
    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        private readonly InternV2DbContext _internV2DbContext;

        private readonly DbSet<TEntity> _dbSet;
        private readonly IList<string> _navigationProperties;

        public virtual IQueryable<TEntity> Queryable => _dbSet.AsQueryable();
        public virtual IUnitOfWork UnitOfWork => _internV2DbContext;

        public BaseRepository(InternV2DbContext internV2DbContext)
        {
            _internV2DbContext = internV2DbContext;
            _dbSet = internV2DbContext.Set<TEntity>();
            _navigationProperties = typeof(TEntity).GetProperties()
                .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                .Select(p => p.Name).ToList();
        }

        public async Task<TEntity?> GetbyId(Guid id)
        {
            var entity = await Queryable.FirstOrDefaultAsync(a => a.Id == id).ConfigureAwait(continueOnCapturedContext: false);
            return entity;
        }
        public async Task<TEntity?> GetById(Guid id)
        {
            var query = Queryable;

            foreach (var navigationProperty in _navigationProperties)
            {
                query = query.Include(navigationProperty);
            }
            return await query.FirstOrDefaultAsync(a => a.Id == id).ConfigureAwait(continueOnCapturedContext: false); ;
        }
        public async Task<TEntity?> GetById(Guid id, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = Queryable;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            var entity = await query.FirstOrDefaultAsync(a => a.Id == id).ConfigureAwait(continueOnCapturedContext: false);
            return entity;
        }

        public async Task<IList<TEntity>?> GetbyIds(IEnumerable<Guid> ids)
        {
            if (ids.Count() == 0)
            {
                return null;
            }
            var entity = await Queryable.Where(a => ids.Contains(a.Id)).ToListAsync().ConfigureAwait(continueOnCapturedContext: false);
            return entity != null && entity.Count > 0 ? entity : null;
        }
        public TEntity? Create(TEntity entity)
        {
            var a = _dbSet.Add(entity);
            return a.Entity;
        }
        public void CreateList(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                _dbSet.Add(entity);
            }
        }
        public TEntity? Update(TEntity entity)
        {
            var a = _dbSet.Add(entity);
            return a.Entity;
        }
        public void UpdateList(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                _dbSet.Update(entity);
            }
        }
        public bool Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
            return true;
        }
        public async Task<TEntity?> CreateAsync(TEntity entity)
        {
            var trans = _internV2DbContext.Database.BeginTransaction();
            try
            {
                var a = _dbSet.Add(entity);
                await UnitOfWork.SaveChangesAsync();
                trans.Commit();
                return a.Entity;

            }
            catch (Exception ex)
            {
                trans.Rollback();
                LogException(ex);
                return null;
            }
            finally
            {
                if (trans != null)
                {
                    trans.Dispose();
                }
            }
        }

        public async Task<TEntity?> UpdateAsync(TEntity entity)
        {
            var trans = _internV2DbContext.Database.BeginTransaction();
            try
            {
                var a = _dbSet.Update(entity);
                await UnitOfWork.SaveChangesAsync();
                trans.Commit();
                return a.Entity;

            }
            catch (Exception ex)
            {
                trans.Rollback();
                LogException(ex);
                return null;
            }
            finally
            {
                if (trans != null)
                {
                    trans.Dispose();
                }
            }
        }
        public async Task<bool> DeleteAsync(TEntity entity)
        {
            var trans = _internV2DbContext.Database.BeginTransaction();
            try
            {
                var a = _dbSet.Remove(entity);
                await UnitOfWork.SaveChangesAsync();
                trans.Commit();
                return true;

            }
            catch (Exception ex)
            {
                trans.Rollback();
                LogException(ex);
                return false;
            }
            finally
            {
                if (trans != null)
                {
                    trans.Dispose();
                }
            }
        }
        private void LogException(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red; // Đổi màu chữ thành đỏ
            Console.WriteLine($"Error Message: {ex.Message}");

            // In chi tiết lỗi nếu cần cho mục đích phát triển
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            if (ex.HelpLink != null)
            {
                Console.WriteLine($"HelpLink: {ex.HelpLink}");
            }
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            Console.ResetColor(); // Đặt lại màu mặc định

        }
        public virtual async Task ExecuteTransactionAsync(Func<Task<ObjectResult>> action, IsolationLevel level = IsolationLevel.Unspecified)
        {
            Func<Task<ObjectResult>> action2 = action;
            ArgumentNullException.ThrowIfNull(action2, "action");
            if (_internV2DbContext.Database.IsInMemory() || _internV2DbContext.HasActiveTransaction)
            {
                await action2().ConfigureAwait(continueOnCapturedContext: false);
                return;
            }

            await _internV2DbContext.Database.CreateExecutionStrategy().ExecuteAsync(async delegate
            {
                using IDbContextTransaction transaction = await _internV2DbContext.BeginTransactionAsync(level).ConfigureAwait(continueOnCapturedContext: false);
                if (transaction != null)
                {
                    try
                    {
                        if ((await action2().ConfigureAwait(continueOnCapturedContext: false))?.StatusCode is >= 200 and <= 204)
                        {
                            transaction.Commit();
                        }
                        else
                        {
                            transaction.Rollback();
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }).ConfigureAwait(continueOnCapturedContext: false);
        }




    }
}
