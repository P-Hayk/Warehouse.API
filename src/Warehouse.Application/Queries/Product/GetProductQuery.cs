using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Domain.Models;

namespace Warehouse.Application.Queries
{
    public class GetProductsQuery : IRequest<GetProductResponse>
    {
    }

    public class GetProductResponse
    {
        public ICollection<Product> Products { get; set; }
    }
}
