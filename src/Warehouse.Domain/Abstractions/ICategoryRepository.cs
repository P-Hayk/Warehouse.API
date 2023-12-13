using Warehouse.Domain.Models;

namespace Warehouse.Domain.Abstractions
{
    public interface ICategoryRepository
    {
        Task<Category> GetAsync(int id);
        Task<ICollection<Category>> GetAllAsync();
        Task<Category> GetByNameAsync(string name);
        Task CreateAsync(Category category);
    }
}
