using Forex.Infrastructure.RabbitMq.Abstractions;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Application.Events;
using Warehouse.Domain.Abstractions;
using Warehouse.Domain.Exceptions;
using Warehouse.Domain.Models;

namespace Warehouse.Application.Commands
{
    public class MakeOrderCommandHandler : IRequestHandler<MakeOrderCommand, Unit>
    {
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IBus _bus;

        public MakeOrderCommandHandler(IBus bus, IProductRepository productRepository, IOrderRepository orderRepository)
        {
            _bus = bus;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }

        public async Task<Unit> Handle(MakeOrderCommand request, CancellationToken cancellationToken)
        {
            request.ProductId = 1;

            var product = await _productRepository.GetProductWithCategoryAsync(request.ProductId);
            if (product is null)
            {
                throw new NotFoundException("Product not found.");
            }

            if (product.State == ProductState.OutOfStock)
            {
                if (!request.ReserveWhenAvaliable)
                {
                    throw new NotFoundException("Product not avaliable.");
                }
            }

            if (product.Stock < request.Count)
            {
                throw new NotFoundException("Product not avaliable.");
            }

            var correlationId = Guid.NewGuid();

            product.Stock--;

            var orderId = await _orderRepository.CreateAsync(new Order
            {
                ClientId = request.ClientId,
                ProductId = product.Id,
                State = OrderState.Pending,
                CorrelationId = correlationId,
                DateTime = DateTime.UtcNow,
                Count = 2,
            });

            var message = new OrderSubmittedEvent
            {
                CorrelationId = correlationId,
                OrderId = orderId,
                OrderState = OrderState.Pending,
                ProductId = product.Id,
                ClientId = request.ClientId,
                Count = request.Count,
                ReserveWhenAvaliable = request.ReserveWhenAvaliable,
                ProductState = product.State,
                DateTime = DateTime.UtcNow
            };


            await _bus.Publish(message, cancellationToken);

            return Unit.Value;
        }
    }
}
