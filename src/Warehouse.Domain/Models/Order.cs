namespace Warehouse.Domain.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ProductId { get; set; }
        public OrderState State { get; set; }
        public DateTime DateTime { get; set; }
        public Guid? CorrelationId { get; set; }
        public int Count { get; set; }
        public Product Product { get; set; }
        public Client Client { get; set; }
    }

    public enum OrderState
    {
        Approved = 1,
        Rejected = 2,
        UnderReview = 3,
        WaitingAvailability = 4,
    }
}
