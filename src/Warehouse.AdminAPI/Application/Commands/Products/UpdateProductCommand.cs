using MediatR;

namespace Warehouse.AdminAPI.Application.Commands.Products
{
    public class UpdateProductCommand : IRequest<Unit>
    {
        public int Id { get; set; }

        public int Stock { get; set; }

        public string Name { get; set; }
    }
}
