using MediatR;

namespace Warehouse.AdminAPI.Application.Commands.Categories
{
    public class CreateCategoryCommand : IRequest<Unit>
    {
        public string Name { get; set; }
        public int StockThreshold { get; set; }
    }
}
