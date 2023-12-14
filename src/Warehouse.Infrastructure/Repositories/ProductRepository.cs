using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Domain.Abstractions;
using Warehouse.Domain.Models;
using Warehouse.Infrastructure.Abstraction;

namespace Warehouse.Infrastructure.Repositories;
public class ProductRepository : IProductRepository
{
    private readonly PostgreDbContext _dbContext;

    public ProductRepository(PostgreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Product> GetAsync(int id)
    {
        return await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<ICollection<Product>> GetAllAsync()
    {
        return await _dbContext.Products.ToListAsync();
    }
    public async Task<Product> GetProductWithCategoryAsync(int productId)
    {
        return await _dbContext.Products.Include(p => p.Category)
                                        .FirstOrDefaultAsync(p => p.Id == productId);

    }

    public async Task UpdateAsync(Product product)
    {
        _dbContext.Update(product);
        await _dbContext.SaveChangesAsync();

    }
}
