﻿using Forex.Infrastructure.RabbitMq.Abstractions;
using MassTransit;
using MediatR;
using Warehouse.AdminAPI.Application.Commands.Categories;
using Warehouse.Application.Events;
using Warehouse.Domain.Abstractions;
using Warehouse.Domain.Models;

namespace Warehouse.AdminAPI.Application.Commands.Orders
{
    public class ApproveOrderCommandHandler : IRequestHandler<ApproveOrderCommand, Unit>
    {
        private readonly IBus _bus;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IEventPublisher _eventPublisher;

        public ApproveOrderCommandHandler(IBus bus, ICategoryRepository categoryRepository
                                        , IProductRepository productRepository
                                        , IOrderRepository orderRepository
            , IEventPublisher eventPublisher
            )
        {
            _bus = bus;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _eventPublisher = eventPublisher;
        }

        public async Task<Unit> Handle(ApproveOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetAsync(request.Id);

            if (order is null)
            {
                throw new ArgumentException("Order not found");
            }

            if (order.State != OrderState.UnderReview)
            {
                throw new ArgumentException("Order not found");
            }

            order.State = OrderState.Approved;
            await _orderRepository.UpdateStateAsync(order);

            var @event = new OrderApprovedEvent
            {
                CorrelationId = order.CorrelationId.Value,
                Order = order
            };
            await _bus.Publish(@event, cancellationToken);

            return Unit.Value;
        }
    }
}