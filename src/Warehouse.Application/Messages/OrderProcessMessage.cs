using MassTransit;
using Warehouse.Domain.Models;

namespace Warehouse.Application.Messages
{
    public class OrderProcessMessage : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public int ClientId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public DateTime DateTime { get; set; }
        public ProductState ProductState { get; set; }
        public int OrderId { get; set; }
        public OrderState OrderState { get; set; }
    }
}
