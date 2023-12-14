namespace Warehouse.AdminAPI.ApiModels.Product
{
    public class UpdateProductApiRequest
    {
        public int Id { get; set; }

        public int Stock { get; set; }

        public string Name { get; set; }
    }
}
