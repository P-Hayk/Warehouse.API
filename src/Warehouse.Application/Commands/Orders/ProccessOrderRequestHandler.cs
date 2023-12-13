using Forex.Infrastructure.RabbitMq.Abstractions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Application.Events;
using Warehouse.Application.Messages;
using Warehouse.Domain.Abstractions;

namespace Warehouse.Application.Commands
{
    public class ProccessOrderRequestHandler : IRequestHandler<ProccessOrderRequest, Unit>
    {
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IEventPublisher _eventPublisher;

        public ProccessOrderRequestHandler(IProductRepository productRepository
                                         , IOrderRepository orderRepository
                                         , IEventPublisher eventPublisher)
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _eventPublisher = eventPublisher;
        }

        public async Task<Unit> Handle(ProccessOrderRequest request, CancellationToken cancellationToken)
        {
            //if (request.Order.Product.State == Domain.Models.ProductState.Available)
            //{
            //    var @event = new OrderApprovedEvent
            //    {
            //        Order = request.Order,
            //    };
            //    await _eventPublisher.Publish(@event, cancellationToken);
            //}

            //if (request.Order.Product.State == Domain.Models.ProductState.OutOfStock)
            //{
            //    var @event = new OrderRejectedEvent { };
            //    await _eventPublisher.Publish(@event, cancellationToken);
            //}

            return Unit.Value;
        }
    }
}
