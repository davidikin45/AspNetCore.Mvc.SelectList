using Microsoft.EntityFrameworkCore;
using System;

namespace Example.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
           : base(options)
        { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscription>().HasData(
                new Subscription
                {
                    Id = "Free",
                    Description = "Free",
                    Cost = 0.00m
                },
                new Subscription
                {
                    Id = "Standard",
                    Description = "Standard",
                    Cost = 50.00m
                },
                new Subscription
                {
                    Id = "Plus",
                    Description = "Plus",
                    Cost = 100.00m
                }
            );

            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = Guid.Parse("ecf1f87a-ce11-471d-abae-735d23c91256"),
                    Name = "Test User"
                }
            );
        }
    }
}
