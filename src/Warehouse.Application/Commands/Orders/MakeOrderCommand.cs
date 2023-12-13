using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Application.Commands
{
    public class MakeOrderCommand : IRequest<Unit>
    {
        public int ProductId { get; set; }
        public int ClientId { get; set; }
        public bool ReserveWhenAvaliable { get; set; }
        public int Count { get; set; }
    }
}
