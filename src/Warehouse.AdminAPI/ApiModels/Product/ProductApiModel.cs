using Warehouse.Domain.Models;

namespace Warehouse.AdminAPI.ApiModels
{
    public class ProductApiModel
    {
        public int Id { get; set; }
        public int Stock { get; set; }
        public string Name { get; set; }
        public ProductState State { get; set; }
    }
}
