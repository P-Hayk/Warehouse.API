using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Domain.Models;

namespace Warehouse.Infrastructure.Abstraction
{
    public interface IDbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Product> Products { get; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Category> Categories { get; set; }

        Task<int> SaveChangesAsync();
        
        int SaveChanges();
    }
}
