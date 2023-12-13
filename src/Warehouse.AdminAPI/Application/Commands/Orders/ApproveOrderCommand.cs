using MediatR;

namespace Warehouse.AdminAPI.Application.Commands.Orders
{
    public class ApproveOrderCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
