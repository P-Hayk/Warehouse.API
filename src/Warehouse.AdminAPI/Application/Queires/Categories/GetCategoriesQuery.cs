using MediatR;
using Warehouse.Domain.Models;

namespace Warehouse.AdminAPI.Application.Queires.Categories
{
    public class GetCategoriesQuery : IRequest<GetCategoryResponse>
    {
    }
 
    public class GetCategoryResponse
    {
        public ICollection<Category> Categories { get; set; }
    }
}
