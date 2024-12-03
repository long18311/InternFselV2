using InternFselV2.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace InternFselV2
{
    public class InternV2DbContext : DbContext
    {
        public InternV2DbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-GSDMP53\\VANLONG;Initial Catalog=InternV2;Integrated Security=True; TrustServerCertificate=true;");
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
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
