using Microsoft.EntityFrameworkCore;
using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Data
{
    public class Dsw2025TpiContext : DbContext
    {
        public Dsw2025TpiContext(DbContextOptions<Dsw2025TpiContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.CurrentUnitPrice)
                .HasPrecision(18, 2);
        }


    }
}
