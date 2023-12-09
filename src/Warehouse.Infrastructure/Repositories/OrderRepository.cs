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
        _dbContext.Orders.Add(order);

        await _dbContext.SaveChangesAsync();
    }

}
