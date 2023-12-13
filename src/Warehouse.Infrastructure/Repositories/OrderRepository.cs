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
public class OrderRepository : IOrderRepository
{
    private readonly IDbContext _dbContext;

    public OrderRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateAsync(Order order)
    {
        order.Product = null;

        _dbContext.Orders.Add(order);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<Order> GetAsync(int id)
    {
        return await _dbContext.Orders.FirstOrDefaultAsync(p => p.ClientId == id);
    }

    public async Task UpdateAsync(Order order)
    {
        _dbContext.Orders.Update(order);
        await _dbContext.SaveChangesAsync();

    }
}
