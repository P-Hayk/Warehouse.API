using Forex.Infrastructure.Kafka.Abstractions;
using Forex.Kafka.Contracts.Vps;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Domain.Abstractions;
using Warehouse.Domain.Exceptions;
using Warehouse.Domain.Models;

namespace Warehouse.Application.Commands
{
    public class MakeOrderCommandHandler : IRequestHandler<MakeOrderCommand, Unit>
    {
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IEventProducer _eventProducer;

        public MakeOrderCommandHandler(IEventProducer eventProducer, IProductRepository productRepository, IOrderRepository orderRepository)
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _eventProducer = eventProducer;
        }

        public async Task<Unit> Handle(MakeOrderCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProductWithCategoryAsync(request.ProductId);
            if (product is null)
            {
                throw new NotFoundException("Product not found.");
            }

            Order order = new Order();

            if (product.State == Domain.Models.ProductState.OutOfStock)
            {
                if (!request.ReserveWhenAvaliable)
                {
                    throw new NotFoundException("Product not avaliable.");
                }

                order.State = OrderState.Pending;
            }
            else if (product.State == ProductState.LowStock)
            {
                order.State = OrderState.UnderReview;
            }
            else
            {
                order.State = OrderState.Approved;
            }

            order.ProductId = product.Id;
            order.ClientId = 1;
            order.DateTime = DateTime.UtcNow;

            await _orderRepository.CreateAsync(order);

            var vpsServiceChangedEvent = new VpsServiceChangedEvent
            {
                Id = request.ProductId,
                ClientId = request.ClientId,
                //ProductWalletId = subscription.ProductWalletId,
                //EventData = new VpsServiceChangedEvent.Types.EventData { Cancelled = new VpsServiceChangedEvent.Types.EventData.Types.Cancelled() },
                //Revision = subscription.Revision,
                //EventDateUtc = Timestamp.FromDateTime(_clock.UtcNow)
            };
            await _eventProducer.ProduceAsync(request.ProductId, vpsServiceChangedEvent, cancellationToken);


            return Unit.Value;
        }
    }
}
