using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.AdminAPI.ApiModels;
using Warehouse.AdminAPI.Application.Commands.Orders;
using Warehouse.Application.Queries;

namespace Warehouse.AdminAPI.Controllers
{
    public class OrderController : ControllerBase
    {
        private readonly ISender _sender;

        public OrderController(ISender sender)
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
                Stock = x.Stock,
                State = x.State
            });

            return Ok(products);
        }

        [HttpPut("approve")]
        public async Task<IActionResult> Approve()
        {
            var response = await _sender.Send(new ApproveOrderCommand());

           
            return Ok(response);
        }
    }
}
