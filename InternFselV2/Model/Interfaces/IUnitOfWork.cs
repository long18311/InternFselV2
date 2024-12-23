namespace InternFselV2.Model.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync();

        //Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        //Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess = true, CancellationToken cancellationToken = default(CancellationToken));

        //Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess = true, bool isSoftDelete = true, CancellationToken cancellationToken = default(CancellationToken));

        //Task SaveEntitiesAsync();

        //Task<Guid> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));

        //Task<Guid> SaveEntitiesAsync(bool acceptAllChangesOnSuccess = true, CancellationToken cancellationToken = default(CancellationToken));

        //Task<Guid> SaveEntitiesAsync(bool acceptAllChangesOnSuccess = true, bool isSoftDelete = true, CancellationToken cancellationToken = default(CancellationToken));
    }
}
