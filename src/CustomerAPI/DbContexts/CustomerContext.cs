using CustomerAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerAPI.DbContexts
{
    public class CustomerContext : DbContext
    {
        public CustomerContext(DbContextOptions<CustomerContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>().Property(c => c.FirstName).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Customer>().Property(c => c.LastName).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Customer>().Property(c => c.DateOfBirth).IsRequired();
        }
        public DbSet<Customer> Customers { get; set; }
    }
}
