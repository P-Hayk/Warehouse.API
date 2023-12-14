using MediatR;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Warehouse.AdminAPI.ApiModels;
using Warehouse.AdminAPI.ApiModels.Product;
using Warehouse.AdminAPI.Application.Commands.Products;
using Warehouse.Application.Queries;

namespace Warehouse.AdminAPI.Controllers
{

    [ApiController]
    [Route("products")]
    public class ProductController : ControllerBase
    {
        private readonly ISender _sender;

        public ProductController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> Products()
        {
            var response = await _sender.Send(new GetProductsQuery());

            var products = response.Products.Select(x => new ProductApiModel
            {
                Id = x.Id,
                Name = x.Name,
                Stock = x.Stock
            });

            return Ok(products);


        }

        [HttpPost]
        public async Task<IActionResult> Create()
        {
            var response = await _sender.Send(new GetProductsQuery());

            var products = response.Products.Select(x => new ProductApiModel
            {
                Id = x.Id,
                Name = x.Name,
                Stock = x.Stock
            });

            return Ok(products);
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateProductApiRequest request)
        {
            var response = await _sender.Send(new UpdateProductCommand
            {
                Id = request.Id,
                Name = request.Name,
                Stock = request.Stock
            });

            return Ok(response);
        }
    }
}
