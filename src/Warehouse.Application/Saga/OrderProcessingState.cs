using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Domain.Models;

namespace Warehouse.Application.Saga
{
    public class OrderProcessingState : SagaStateMachineInstance//, IVersionedSaga
    {
        public OrderProcessingState(Guid correlationId)
        {
            this.CorrelationId = correlationId;
        }
        public Guid Id { get; set; }
        public Order Order { get; set; }      
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Guid CorrelationId { get; set; }

        public string State { get; set; }
        public int Version { get; set; }
    }
}
