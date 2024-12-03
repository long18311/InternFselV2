using InternFselV2.Entities;
using InternFselV2.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace InternFselV2.Repositories.Repositories
{
    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        private readonly InternV2DbContext _internV2DbContext;

        private readonly DbSet<TEntity> _dbSet;

        public virtual IQueryable<TEntity> Queryable { get { return _dbSet.AsQueryable(); } }

        public BaseRepository(InternV2DbContext internV2DbContext)
        {
            _internV2DbContext = internV2DbContext;
            _dbSet = internV2DbContext.Set<TEntity>();
        }

        public async Task<TEntity?> GetbyId(Guid id)
        {
            var entity = await Queryable.SingleOrDefaultAsync(a => a.Id == id);
            return entity;
        }
        public async Task<IList<TEntity>?> GetbyIds(IEnumerable<Guid> ids)
        {
            if(ids.Count() == 0)
            {
                return null;
            }
            var entity = await Queryable.Where(a => ids.Contains(a.Id)).ToListAsync();
            return entity != null && entity.Count > 0 ? entity : null;
        }
        public async Task<TEntity?> Create(TEntity entity)
        {
            var trans = _internV2DbContext.Database.BeginTransaction();
            try
            {
                var a = _dbSet.Add(entity);
                await _internV2DbContext.SaveChangesAsync();
                trans.Commit();
                return await Queryable.SingleOrDefaultAsync(x => x.Id == a.Entity.Id);

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

        public async Task<TEntity?> Update(TEntity entity)
        {
            var trans = _internV2DbContext.Database.BeginTransaction();
            try
            {
                var a = _dbSet.Update(entity);
                await _internV2DbContext.SaveChangesAsync();
                trans.Commit();
                return await Queryable.SingleOrDefaultAsync(x => x.Id == a.Entity.Id);

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
        public async Task<bool> DeleteById(Guid id)
        {
            var entity = await this.GetbyId(id);
            if(entity == null)
            {
                return false;
            }
            return await this.Delete(entity);
        }
        public async Task<bool> Delete(TEntity entity)
        {
            var trans = _internV2DbContext.Database.BeginTransaction();
            try
            {
                var a = _dbSet.Remove(entity);
                await _internV2DbContext.SaveChangesAsync();
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
    }
}
