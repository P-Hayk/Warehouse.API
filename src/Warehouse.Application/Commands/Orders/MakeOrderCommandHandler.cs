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
            var product = await _productRepository.GetProductWithCategoryAsync(request.ProductId);
            if (product is null)
            {
                throw new NotFoundException("Product not found.");
            }

            Order order = new Order { Product = product , ProductId = product.Id };

            if (product.State == Domain.Models.ProductState.OutOfStock)
            {
                if (!request.ReserveWhenAvaliable)
                {
                    throw new NotFoundException("Product not avaliable.");
                }

                order.State = OrderState.Pending;
            }
            //else if (product.State == ProductState.LowStock)
            //{
            //    order.State = OrderState.UnderReview;
            //}
            //else
            //{
            //    order.State = OrderState.Approved;
            //}

            order.ClientId = 1;
            order.DateTime = DateTime.UtcNow;

            var message = new OrderSubmittedEvent { CorrelationId = Guid.NewGuid(), Order = order };

            await _bus.Publish(message, cancellationToken);


            return Unit.Value;
        }
    }
}
