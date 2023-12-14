using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Domain.Models;
using Warehouse.Infrastructure.Abstraction;
using Warehouse.Infrastructure.Extensions;

namespace Warehouse.Infrastructure;

public class PostgreDbContext : DbContext
{
    public PostgreDbContext(DbContextOptions<PostgreDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //----------
        var categoryEntity = modelBuilder.Entity<Category>();

        categoryEntity.ToTable("Categories")
                      .HasKey(e => e.Id);

        categoryEntity.Property(e => e.Id)
                      .ValueGeneratedOnAdd();

        categoryEntity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(255);

        categoryEntity.Property(e => e.StockThreshold)
                      .IsRequired();

        categoryEntity.HasData(new DbSeedData().Categories);

        //----------
        var productEntity = modelBuilder.Entity<Product>();

        productEntity.ToTable("Products")
                     .HasKey(e => e.Id);

        productEntity.Property(e => e.Id)
                     .ValueGeneratedOnAdd();

        productEntity.Property(e => e.Name)
                     .IsRequired()
                     .HasMaxLength(255);

        productEntity.Property(e => e.Stock)
                     .IsRequired();

        productEntity.Ignore(e => e.State);

        productEntity.HasOne(e => e.Category)
                     .WithMany()
                     .HasForeignKey(c => c.CategoryId);

        productEntity.HasData(new DbSeedData().Products);

        //----------
        var orderEntity = modelBuilder.Entity<Order>();

        orderEntity.ToTable("Orders")
                   .HasKey(e => e.Id);

        orderEntity.Property(e => e.Id)
                   .ValueGeneratedOnAdd();

        orderEntity.HasOne(e => e.Product)
                   .WithMany()
                   .HasForeignKey(o => o.ProductId);

        orderEntity.HasOne(e => e.Client)
                   .WithMany(e => e.Orders);

        orderEntity.Property(e => e.DateTime)
                   .IsRequired();

        orderEntity.Property(e => e.State)
                   .IsRequired();

        orderEntity.Property(e => e.Count)
                  .IsRequired();

        orderEntity.Property(e => e.CorrelationId)
                   .IsRequired(false);
    }


    public DbSet<Product> Products { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Category> Categories { get; set; }

    public async Task<int> SaveChangesAsync()
    {
        return await base.SaveChangesAsync();
    }
}