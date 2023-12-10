﻿using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Application.Events
{
    public class OrderProccessedEvent : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; }

    }
}
