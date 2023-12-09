using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Queries;
using Warehouse.ClientAPI.ApiModels;

namespace Warehouse.ClientAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ISender _sender;

        public ProductController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            var response = await _sender.Send(new GetProductsQuery());

            var products = response.Products.Select(x => new ProductApiModel
            {
                Id = x.Id
            });

            return Ok(products);
        }
    }
}
