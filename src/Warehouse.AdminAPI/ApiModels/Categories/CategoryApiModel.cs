namespace Warehouse.AdminAPI.ApiModels.Categories
{
    public class CategoryApiModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StockThreshold { get; set; }
    }
}
