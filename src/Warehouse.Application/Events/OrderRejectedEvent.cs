using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Domain.Models;

namespace Warehouse.Application.Events
{
    public class OrderRejectedEvent : CorrelatedBy<Guid>
    {
        public Order Order { get; set; }

        public Guid CorrelationId { get; set; }
    }
}
