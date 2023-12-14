using Forex.Infrastructure.RabbitMq.Abstractions;
using MassTransit;
using Warehouse.Application.Events;
using Warehouse.Application.Messages;
using Warehouse.Domain.Abstractions;
using Warehouse.Domain.Models;

namespace Warehouse.Application.Consumers
{
    public class OrderReserveConsumer : IConsumer<OrderReserveMessage>
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly IOrderRepository _orderRepository;
        private readonly IBus _bus;

        public OrderReserveConsumer(IBus bus, IEventPublisher eventPublisher, IOrderRepository orderRepository)
        {
            _eventPublisher = eventPublisher;
            _bus = bus;
            _orderRepository = orderRepository;
        }

        public async Task Consume(ConsumeContext<OrderReserveMessage> context)
        {
            var (message, cancellationToken) = (context.Message, context.CancellationToken);

            var @event = new OrderReservedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                Order = message.Order,
            };

            await context.Publish(@event, cancellationToken);

        }
    }
}
