using MassTransit;
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
            InstanceState(x => x.State);
            State(() => Pending);
            Initially(
                 When(OrderSubmitted).Then(c => this.UpdateSagaState(c.Saga, c.Saga.Order))
                               .Publish(c => new OrderProcessMessage
                               {
                                   CorrelationId = c.Saga.CorrelationId,
                                   OrderId = c.Message.OrderId,
                                   ClientId = c.Message.ClientId,
                                   ProductId = c.Message.ProductId,
                                   DateTime = c.Message.DateTime,
                                   Count = c.Message.Count,
                                   OrderState = c.Message.OrderState,
                                   ReserveWhenAvaliable = c.Message.ReserveWhenAvaliable

                               }).TransitionTo(Pending)
                               );

            During(
                Pending,
                When(OrderApproved).Then(c => this.UpdateSagaState(c.Saga, c.Saga.Order))
                                   .TransitionTo(Final),

                When(OrderReserved).Then(c => this.UpdateSagaState(c.Saga, c.Saga.Order))
                                   .TransitionTo(WaitingForAvailableStock),
                                    Ignore(OrderSubmitted)
                                    );

            During(
                WaitingForAvailableStock,
                When(OrderApproved).Then(c => this.UpdateSagaState(c.Saga, c.Saga.Order))
                                   .TransitionTo(Final),

                When(OrderRejected).Then(c => this.UpdateSagaState(c.Saga, c.Saga.Order))
                                   .TransitionTo(Final)
                                   );

            SetCompletedWhenFinalized();
        }

        private void UpdateSagaState(OrderProcessingState state, Order order)
        {
            var currentDate = DateTime.UtcNow;
            state.Created = currentDate;
            state.Updated = currentDate;
            state.Order = order;
        }


        public State Pending { get; private set; }
        public State WaitingForAvailableStock { get; private set; }
        public Event<OrderSubmittedEvent> OrderSubmitted { get; private set; }
        public Event<OrderApprovedEvent> OrderApproved { get; private set; }
        public Event<OrderRejectedEvent> OrderRejected { get; private set; }
        public Event<OrderReservedEvent> OrderReserved { get; private set; }
    }
}
