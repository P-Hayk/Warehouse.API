using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Domain.Models;
using Warehouse.Infrastructure.Saga;

namespace Warehouse.Infrastructure
{
    public class OrderStateDbContext : SagaDbContext

    {
        public OrderStateDbContext(DbContextOptions<OrderStateDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Order>();
            modelBuilder.Ignore<Category>();
            modelBuilder.Ignore<Client>();
            modelBuilder.Ignore<Product>();
        }

        public DbSet<OrderProcessingState> OrderProcessingSagaState { get; set; }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new OrderStateMap(); }
        }
    }

    public class OrderStateMap : SagaClassMap<OrderProcessingState>
    {
        protected override void Configure(EntityTypeBuilder<OrderProcessingState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState).HasMaxLength(64);
            entity.Property(x => x.OrderDate);

            // If using Optimistic concurrency, otherwise remove this property
            //entity.Property(x => x.RowVersion).IsRowVersion();
        }
    }


}
