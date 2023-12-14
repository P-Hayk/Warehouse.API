using MediatR;

namespace Warehouse.AdminAPI.Application.Commands.Orders
{
    public class RejectOrderCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}