using MassTransit;
using MediatR;
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


            var dateTime = DateTime.UtcNow;

            var order = new Order
            {
                ClientId = request.ClientId,
                ProductId = product.Id,
                DateTime = dateTime,
                Count = request.Count
            };


            if (product.State == ProductState.OutOfStock || product.Stock < request.Count)
            {
                if (!request.ReserveWhenAvaliable)
                {
                    throw new NotFoundException("Product not avaliable.");
                }
                else
                {
                    order.State = OrderState.WaitingAvailability;

                    order.CorrelationId = Guid.NewGuid();

                    var orderId = await _orderRepository.CreateAsync(order);

                    var message = new OrderSubmittedEvent
                    {
                        CorrelationId = order.CorrelationId.Value,
                        OrderId = orderId,
                        OrderState = order.State,
                        ProductId = product.Id,
                        ClientId = request.ClientId,
                        Count = request.Count,
                        DateTime = dateTime
                    };

                    await _bus.Publish(message, cancellationToken);
                }

                return Unit.Value;
            }

            order.State = product.State == ProductState.Available ? OrderState.Approved : OrderState.UnderReview;

            product.Stock -= request.Count;

            await _orderRepository.CreateAsync(order);

            return Unit.Value;
        }
    }
}
