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
public class CategoryRepository : ICategoryRepository
{
    private readonly PostgreDbContext _dbContext;

    public CategoryRepository(PostgreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Category> GetAsync(int id)
    {
        return await _dbContext.Categories.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<ICollection<Category>> GetAllAsync()
    {
        return await _dbContext.Categories.ToListAsync();
    }

    public async Task<Category> GetByNameAsync(string name)
    {
        return await _dbContext.Categories.FirstOrDefaultAsync(p => p.Name == name);
    }

    public async Task CreateAsync(Category category)
    {
        _dbContext.Categories.Add(category);

        await _dbContext.SaveChangesAsync();
    }
}
