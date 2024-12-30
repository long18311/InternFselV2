using InternFselV2.Entities;
using InternFselV2.Model.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using System.Data;
using System.Threading;

namespace InternFselV2
{
    public class InternV2DbContext : DbContext , IUnitOfWork
    {
        private IDbContextTransaction? _currentTransaction;
        public bool HasActiveTransaction => _currentTransaction != null;
        public IDbContextTransaction? GetCurrentTransaction()
        {
            return _currentTransaction;
        }
        public InternV2DbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>()
            .HasOne(p => p.CreatedUser)
            .WithMany(u => u.Products)
            .HasForeignKey(p => p.CreatedUserId)
            .OnDelete(DeleteBehavior.SetNull);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync(true, default(CancellationToken));
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel level = IsolationLevel.Unspecified)
        {
            if (_currentTransaction != null)
            {
                _currentTransaction = null;
            }

            _currentTransaction = await Database.BeginTransactionAsync(level);
            return _currentTransaction;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
