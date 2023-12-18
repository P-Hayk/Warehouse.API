namespace Warehouse.AdminAPI.ApiModels.Categories
{
    public class CategoryRequest
    {
        public string Name { get; set; }
        public int StockThreshold { get; set; }
    }
}
