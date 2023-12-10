using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Warehouse.Application.Events;
using Warehouse.Application.Messages;
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
            this.Initially(SetOrderSubmittedEventHandler());
            this.During(
                Pending
               , SetOrderSubmittedEventHandler()
               , SetOrderApprovedEventHandler()
               , SetStockReservedHandler()
               , SetOrderProcessedHandler()
               );
            SetCompletedWhenFinalized();
        }

        private void ConfigureCorrelationIds()
        {
            this.Event(() => this.OrderSubmitted
            //, x => x.CorrelateById(c => c.Message.CorrelationId)
            );

            this.Event(() => this.OrderProcessed
            //, x => x.CorrelateById(c => c.Message.CorrelationId)
            );

            this.Event(() => this.OrderReserved
            //, x => x.CorrelateById(c => c.Message.CorrelationId)
            );

            this.Event(() => this.OrderApproved
           //, x => x.CorrelateById(c => c.Message.CorrelationId)
           );

            this.Event(() => this.OrderRejected
         //, x => x.CorrelateById(c => c.Message.CorrelationId)
         );
        }


        private EventActivityBinder<OrderProcessingState, OrderSubmittedEvent> SetOrderSubmittedEventHandler() =>
            When(OrderSubmitted).Then(c => this.UpdateSagaState(c.Saga, c.Saga.Order))
                               .Publish(c => new OrderProcessMessage
                               {
                                   CorrelationId = c.Saga.CorrelationId,
                                   Order = c.Message.Order

                               }).TransitionTo(Pending);


        private EventActivityBinder<OrderProcessingState, OrderApprovedEvent> SetOrderApprovedEventHandler() =>
            When(OrderApproved).Then(c => this.UpdateSagaState(c.Saga, c.Saga.Order))
                               .Finalize();

        private EventActivityBinder<OrderProcessingState, OrderReservedEvent> SetStockReservedHandler() =>
            When(OrderReserved).Then(c => this.UpdateSagaState(c.Saga, c.Saga.Order))
                               .Finalize();

        private EventActivityBinder<OrderProcessingState, OrderProccessedEvent> SetOrderProcessedHandler() =>
            When(OrderProcessed).Then(c => this.UpdateSagaState(c.Saga, c.Saga.Order))
                                .Finalize();


        private void UpdateSagaState(OrderProcessingState state, Order order)
        {
            var currentDate = DateTime.UtcNow;
            state.Created = currentDate;
            state.Updated = currentDate;
            state.Order = order;
        }

        //private async Task SendCommand<TCommand>(string endpointKey, BehaviorContext<OrderProcessingState, IMessage> context)
        //    where TCommand : class, IMessage
        //{
        //    var sendEndpoint = await context.GetSendEndpoint(new Uri(ConfigurationManager.AppSettings[endpointKey]));
        //    await sendEndpoint.Send<TCommand>(new
        //    {
        //        CorrelationId = context.Data.CorrelationId,
        //        Order = context.Data.Order
        //    });
        //}
        public State Pending { get; private set; }

        public Event<OrderSubmittedEvent> OrderSubmitted { get; private set; }
        public Event<OrderApprovedEvent> OrderApproved { get; private set; }
        public Event<OrderRejectedEvent> OrderRejected { get; private set; }
        public Event<OrderProccessedEvent> OrderProcessed { get; private set; }
        public Event<OrderReservedEvent> OrderReserved { get; private set; }
    }
}
