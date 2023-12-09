using Automatonymous;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Application.Events;
using Warehouse.Domain.Models;

namespace Warehouse.Application.Saga
{
    public class OrderProcessorStateMachine : MassTransitStateMachine<OrderProcessingState>
    {
        //private readonly ILog logger;

        public OrderProcessorStateMachine()
        {
            // this.logger = LogManager.GetLogger<OrderProcessorStateMachine>();
            this.InstanceState(x => x.State);
            this.State(() => this.Pending);
            this.ConfigureCorrelationIds();
            this.Initially(this.SetOrderProcessedHandler());
            this.During(Pending, this.SetStockReservedHandler(), SetPaymentProcessedHandler(), SetOrderShippedHandler());
            SetCompletedWhenFinalized();
        }

        private void ConfigureCorrelationIds()
        {
            this.Event(() => this.OrderProcessed, x => x.CorrelateById(c => c.Message.CorrelationId).SelectId(c => c.Message.CorrelationId));
            this.Event(() => this.StockReserved, x => x.CorrelateById(c => c.Message.CorrelationId));
            this.Event(() => this.PaymentProcessed, x => x.CorrelateById(c => c.Message.CorrelationId));
            this.Event(() => this.OrderShipped, x => x.CorrelateById(c => c.Message.CorrelationId));
        }

        private EventActivityBinder<OrderProcessingState, OrderProcessedEvent> SetOrderProcessedHandler() =>
            When(OrderProcessed).Then(c => this.UpdateSagaState(c.Saga, c.Saga.Order))
                                //.Then(c => this.logger.Info($"Order submitted to {c.Data.CorrelationId} received"))
                                .Publish(context => new
                                {
                                    CorrelationId = context.Message.CorrelationId
                                })
                                .TransitionTo(Pending);


        //.ThenAsync(c => this.SendCommand<IReserveStock>("rabbitWarehouseQueue", c))
        //.TransitionTo(Processing);


        private EventActivityBinder<OrderProcessingState, IStockReserved> SetStockReservedHandler() =>
            When(StockReserved).Then(c => this.UpdateSagaState(c.Instance, c.Data.Order))
                               .Then(c => this.logger.Info($"Stock reserved to {c.Data.CorrelationId} received"))

                               .ThenAsync(c => this.SendCommand<IProcessPayment>("rabbitCashierQueue", c));


        private EventActivityBinder<OrderProcessingState, IPaymentProcessed> SetPaymentProcessedHandler() =>
            When(PaymentProcessed).Then(c => this.UpdateSagaState(c.Instance, c.Data.Order))
                                  .Then(c => this.logger.Info($"Payment processed to {c.Data.CorrelationId} received"))
                                  .ThenAsync(c => this.SendCommand<IShipOrder>("rabbitDispatcherQueue", c));


        private EventActivityBinder<OrderProcessingState, IOrderShipped> SetOrderShippedHandler() =>
            When(OrderShipped).Then(c =>
                                        {
                                            this.UpdateSagaState(c.Instance, c.Data.Order);
                                            c.Instance.Order.Status = Status.Processed;
                                        })
                              .Publish(c => new OrderProcessed(c.Data.CorrelationId, c.Data.Order))
                              .Finalize();

        private void UpdateSagaState(OrderProcessingState state, Order order)
        {
            var currentDate = DateTime.Now;
            state.Created = currentDate;
            state.Updated = currentDate;
            state.Order = order;
        }

        private async Task SendCommand<TCommand>(string endpointKey, BehaviorContext<OrderProcessingState, IMessage> context)
            where TCommand : class, IMessage
        {
            var sendEndpoint = await context.GetSendEndpoint(new Uri(ConfigurationManager.AppSettings[endpointKey]));
            await sendEndpoint.Send<TCommand>(new
            {
                CorrelationId = context.Data.CorrelationId,
                Order = context.Data.Order
            });
        }
        public State Pending { get; private set; }

        public Event<OrderProcessedEvent> OrderProcessed { get; private set; }
        public Event<IOrderShipped> OrderShipped { get; set; }
        public Event<IPaymentProcessed> PaymentProcessed { get; private set; }
        public Event<IStockReserved> StockReserved { get; private set; }
    }
}
