using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderService.Model;

namespace OrderService.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }
        public OrderDbContext() { }

        public virtual DbSet<Order> ordersTable { get; set; }
        public virtual DbSet<Product> ProductsTables{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Product)       // One Order has one Product
                .WithMany()                   // One Product can be associated with many Orders
                .HasForeignKey(o => o.ProductId) 
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1, ProductName = "Product 1", ProductDescription = "Product 1 Description" },
                new Product { ProductId = 2, ProductName = "Product 2", ProductDescription = "Product 2 Description" },
                new Product { ProductId = 3, ProductName = "Product 3", ProductDescription = "Product 3 Description" }
            );

            modelBuilder.Entity<Order>().HasData(
                new Order { OrderId = 1, ProductId = 1, VisableFlag = true },
                new Order { OrderId = 2, ProductId = 2, VisableFlag = true },
                new Order { OrderId = 3, ProductId = 3, VisableFlag = true }
            );
        }
    }
}