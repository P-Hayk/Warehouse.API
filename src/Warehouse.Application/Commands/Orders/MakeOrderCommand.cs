using MediatR;

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
