namespace Warehouse.ClientAPI.ApiModels
{
    public class MakeOrderRequest
    {
        public int ProductId { get; set; }
        public bool ReserveWhenAvaliable { get; set; }

        public int Count { get; set; }
    }
}
