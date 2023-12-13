using Forex.Infrastructure.RabbitMq.Abstractions;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Warehouse.Application.Commands;
using Warehouse.Application.Events;
using Warehouse.Application.Messages;
using Warehouse.Domain.Abstractions;
using Warehouse.Domain.Models;
using Warehouse.Infrastructure.Repositories;

namespace Warehouse.Application.Consumers
{
    public class OrderProcessConsumer : IConsumer<OrderProcessMessage>
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly IOrderRepository _orderRepository;
        private readonly IBus _bus;

        public OrderProcessConsumer(IBus bus, IEventPublisher eventPublisher, IOrderRepository orderRepository)
        {
            _eventPublisher = eventPublisher;
            _bus = bus;
            _orderRepository = orderRepository;
        }

        public async Task Consume(ConsumeContext<OrderProcessMessage> context)
        {
            var (message, cancellationToken) = (context.Message, context.CancellationToken);

            var order = new Order
            {
                ClientId = 1,
                ProductId = message.ProductId,
            };

            if (message.ProductState == ProductState.Available)
            {
                order.State = OrderState.Approved;

                var @event = new OrderApprovedEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    Order = order,
                };

                await context.Publish(@event, cancellationToken);
            }

            else if (message.ProductState == ProductState.LowStock)
            {
                order.State = OrderState.UnderReview;

                var @event = new OrderReservedEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    Order = order
                };

                await context.Publish(@event);
            }

            await _orderRepository.UpdateAsync(order);
        }
    }
}
