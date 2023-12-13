using MassTransit;
using Warehouse.Domain.Models;

namespace Warehouse.Application.Messages
{
    public class OrderReserveMessage : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public Order Order { get; set; }
    }
}
