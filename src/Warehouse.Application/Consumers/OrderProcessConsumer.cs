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
using Warehouse.Infrastructure.Repositories;

namespace Warehouse.Application.Consumers
{
    public class OrderProcessConsumer : IConsumer<OrderProcessMessage>
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly IOrderRepository _orderRepository;

        public OrderProcessConsumer(IEventPublisher eventPublisher, IOrderRepository orderRepository)
        {
            _eventPublisher = eventPublisher;
            _orderRepository = orderRepository;
        }

        public IOrderRepository OrderRepository { get; }

        public async Task Consume(ConsumeContext<OrderProcessMessage> context)
        {
            var (order, cancellationToken) = (context.Message.Order, context.CancellationToken);

            if (order.Product.State == Domain.Models.ProductState.Available)
            {
                order.State = Domain.Models.OrderState.Approved;

                var @event = new OrderApprovedEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    Order = order,
                };

                await context.Publish(@event, cancellationToken);

            }

            else if (order.Product.State == Domain.Models.ProductState.OutOfStock)
            {
                var @event = new OrderRejectedEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    Order = order,
                };
                await context.Publish(@event, cancellationToken);
            }

            else if (order.Product.State == Domain.Models.ProductState.LowStock)
            {
                var @event = new OrderReservedEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    Order = order,
                };
                await context.Publish(@event, cancellationToken);
            }

            await _orderRepository.CreateAsync(order);



            //await context.Publish(@event, cancellationToken);

        }
    }
}
