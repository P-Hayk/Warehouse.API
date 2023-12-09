using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Application.Events;

namespace Warehouse.Application.EventHandlers
{
    public class OrderProcessedEventHandler : IConsumer<OrderProcessedEvent>
    {
        public Task Consume(ConsumeContext<OrderProcessedEvent> context)
        {
            throw new NotImplementedException();
        }
    }
}
