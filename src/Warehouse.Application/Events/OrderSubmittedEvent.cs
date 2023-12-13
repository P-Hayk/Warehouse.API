using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Domain.Models;

namespace Warehouse.Application.Events
{
    public class OrderSubmittedEvent : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public int ClientId { get; set; }
        public int ProductId { get; set; }
        public bool ReserveWhenAvaliable { get; set; }
        public int Count { get; set; }
        public DateTime DateTime { get; set; }
        public ProductState ProductState { get; set; }
    }
}
