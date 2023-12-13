using MediatR;
using Warehouse.Application.Queries;
using Warehouse.Domain.Abstractions;
using Warehouse.Domain.Models;

namespace Warehouse.AdminAPI.Application.Queires.Categories
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, GetCategoryResponse>
    {
        private readonly ICategoryRepository _categoryRepository;

        public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<GetCategoryResponse> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetAllAsync();

            return new GetCategoryResponse { Categories = categories };
        }
    }
}
