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
            this.Initially(
                 When(OrderSubmitted).Then(c => this.UpdateSagaState(c.Saga, c.Saga.Order))
                               .Publish(c => new OrderProcessMessage
                               {
                                   CorrelationId = c.Saga.CorrelationId,
                                   ClientId = c.Message.ClientId,
                                   ProductId = c.Message.ProductId,
                                   DateTime = c.Message.DateTime,
                                   Count = c.Message.Count,
                                   ReserveWhenAvaliable = c.Message.ReserveWhenAvaliable

                               }).TransitionTo(Pending)
                );

            this.During(
                Pending,
                When(OrderApproved).Then(c => this.UpdateSagaState(c.Saga, c.Saga.Order))
                               .TransitionTo(Final),
                When(OrderReserved).Then(c => this.UpdateSagaState(c.Saga, c.Saga.Order))
                               .TransitionTo(Processing),
                Ignore(OrderSubmitted)
               );

            this.During(
                Processing,
                When(OrderApproved).Then(c => this.UpdateSagaState(c.Saga, c.Saga.Order))
                               .TransitionTo(Final)
                //When(OrderReserved).Then(c => this.UpdateSagaState(c.Saga, c.Saga.Order))
                //               .TransitionTo(Processing)
               );

            SetCompletedWhenFinalized();
        }

       

        private EventActivityBinder<OrderProcessingState, OrderSubmittedEvent> SetOrderSubmittedEventHandler() =>
            When(OrderSubmitted).Then(c => this.UpdateSagaState(c.Saga, c.Saga.Order))
                               .Publish(c => new OrderProcessMessage
                               {
                                   CorrelationId = c.Saga.CorrelationId,
                                   ClientId = c.Message.ClientId,
                                   ProductId = c.Message.ProductId,
                                   DateTime = c.Message.DateTime,
                                   Count = c.Message.Count,
                                   ReserveWhenAvaliable = c.Message.ReserveWhenAvaliable

                               }).TransitionTo(Pending);


        private EventActivityBinder<OrderProcessingState, OrderApprovedEvent> SetOrderApprovedEventHandler() =>
            When(OrderApproved).Then(c => this.UpdateSagaState(c.Saga, c.Saga.Order))

                               .Finalize();

        private EventActivityBinder<OrderProcessingState, OrderReserveMessage> SetStockReserveHandler() =>
            When(OrderReserve).Then(c => this.UpdateSagaState(c.Saga, c.Message.Order))
                                 .Publish(c => new OrderReserveMessage
                                 {
                                     CorrelationId = c.Saga.CorrelationId,
                                     Order = c.Message.Order
                                 }).TransitionTo(Processing);

        private EventActivityBinder<OrderProcessingState, OrderReservedEvent> SetStockReservedHandler() =>

                  When(OrderReserved).Finalize();



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


        public State Pending { get; private set; }
        public State Processing { get; private set; }
        public Event<OrderSubmittedEvent> OrderSubmitted { get; private set; }
        public Event<OrderApprovedEvent> OrderApproved { get; private set; }
        public Event<OrderRejectedEvent> OrderRejected { get; private set; }
        public Event<OrderProccessedEvent> OrderProcessed { get; private set; }
        public Event<OrderReservedEvent> OrderReserved { get; private set; }
        public Event<OrderReserveMessage> OrderReserve { get; private set; }
    }
}
