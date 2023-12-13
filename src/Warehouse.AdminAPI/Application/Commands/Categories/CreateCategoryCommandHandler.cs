using Forex.Infrastructure.RabbitMq.Abstractions;
using MediatR;
using Warehouse.Application.Commands;
using Warehouse.Application.Events;
using Warehouse.Domain.Abstractions;
using Warehouse.Domain.Models;

namespace Warehouse.AdminAPI.Application.Commands.Categories
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Unit>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IEventPublisher _eventPublisher;

        public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IProductRepository productRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
        }

        public async Task<Unit> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByNameAsync(request.Name);

            if (category is not null)
            {
                throw new ArgumentException("Category exists");
            }

            category = new Category
            {
                Name = request.Name,
                StockThreshold = request.StockThreshold
            };

            await _categoryRepository.CreateAsync(category);

            return Unit.Value;
        }
    }
}