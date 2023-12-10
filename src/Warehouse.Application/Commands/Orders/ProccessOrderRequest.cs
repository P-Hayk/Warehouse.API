using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Domain.Models;

namespace Warehouse.Application.Commands
{
    public class ProccessOrderRequest : IRequest<Unit>
    {
        public Order Order { get; set; }
    }
}
