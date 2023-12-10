using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Domain.Models;

namespace Warehouse.Application.Events
{
    public class OrderReservedEvent : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public Order Order { get;   set; }
    }
}
