using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Domain.Abstractions;

namespace Warehouse.Application.Queries
{
    public class GetProductQueryHandler : IRequestHandler<GetProductsQuery, GetProductResponse>
    {
        private readonly IProductRepository _productRepository;

        public GetProductQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<GetProductResponse> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetAllAsync();

            return new GetProductResponse { Products = products };
        }
    }
}
