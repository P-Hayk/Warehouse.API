using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Domain.Models;
using Warehouse.Infrastructure.Abstraction;

namespace Warehouse.Infrastructure.Extensions
{
    public class DbSeedData
    {
        public List<Category> Categories => new List<Category>
        {
            new Category
            {
                Id = 1,
                Name = "Clothing",
                StockThreshold = 5
            },
            new Category
            {
                Id = 2,
                Name = "Shoes",
                StockThreshold = 10
            }
        };

        public List<Product> Products => new List<Product>
        {
            new Product
            {
                        Id = 1,
                        Name = "Jacket",
                        Stock = 10,
                        CategoryId = 1
            },
            new Product
            {
                        Id = 2,
                        Name = "Jeans",
                        Stock = 10,
                        CategoryId = 1
            },
            new Product
            {
                Id = 3,
                        Name = "Sneaker",
                        Stock = 50,
                        CategoryId = 2
            },
            new Product
            {
                Id = 4,
                        Name = "Boot",
                        Stock = 50,
                        CategoryId = 2
            }
        };
    }
}