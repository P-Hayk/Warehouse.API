

using Forex.Infrastructure.RabbitMq.Abstractions;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Server.HttpSys;
using Warehouse.AdminAPI.Application.Commands.Categories;
using Warehouse.AdminAPI.Application.Commands.Products;
using Warehouse.Application.Events;
using Warehouse.Application.Messages;
using Warehouse.Domain.Abstractions;
using Warehouse.Domain.Models;

namespace Warehouse.AdminAPI.Application.Commands.Orders
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly IBus _bus;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IEventPublisher _eventPublisher;

        public UpdateProductCommandHandler(IBus bus, ICategoryRepository categoryRepository
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

        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetAsync(request.Id);
            if (product is null)
            {
                throw new ArgumentException("Order not found");
            }

            var stockInreased = request.Stock > product.Stock;
            product.Stock = request.Stock;

            await _productRepository.UpdateAsync(product);

            if (stockInreased)
            {
                var orders = await _orderRepository.GetUnderReviewOrdersByProductIdAsync(product.Id);

                var events = new List<OrderProcessMessage>();

                foreach (var order in orders)
                {
                    if (order.Count > request.Stock)
                    {
                        continue;
                    }

                    events.Add(new OrderProcessMessage
                    {
                        CorrelationId = order.CorrelationId.Value,
                        OrderId = order.Id
                    });
                }
                await _bus.PublishBatch(events, cancellationToken);
            }

            // await _orderRepository.UpdateStateAsync(order);

            return Unit.Value;
        }
    }
}