using Warehouse.Domain.Models;

namespace Warehouse.Domain.Abstractions
{
    public interface IProductRepository
    {
        Task<ICollection<Product>> GetAllAsync();
        Task<Product> GetAsync(int id);

        Task<Product> GetProductWithCategoryAsync(int productId);

    }
}
