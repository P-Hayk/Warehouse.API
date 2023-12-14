using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Abstractions;
using Warehouse.Domain.Models;
using Warehouse.Infrastructure.Abstraction;

namespace Warehouse.Infrastructure.Repositories;
public class OrderRepository : IOrderRepository
{
    private readonly PostgreDbContext _dbContext;

    public OrderRepository(PostgreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> CreateAsync(Order order)
    {
        order.Product = null;

        _dbContext.Orders.Add(order);

        await _dbContext.SaveChangesAsync();

        return order.Id;
    }

    public async Task<Order> GetAsync(int id)
    {
        return await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<ICollection<Order>> GetUnderReviewOrdersByProductIdAsync(int id)
    {
        return await _dbContext.Orders.Where(o => o.ProductId == id && o.State == OrderState.UnderReview).ToListAsync();
    }

    public async Task UpdateStateAsync(Order order)
    {
        _dbContext.Orders.Attach(order);
        _dbContext.Entry(order).Property(x => x.State).IsModified = true;
        await _dbContext.SaveChangesAsync();

    }
}
