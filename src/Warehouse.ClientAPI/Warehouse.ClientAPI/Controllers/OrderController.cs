using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Commands;
using Warehouse.ClientAPI.ApiModels;
using Warehouse.Domain.Models;

namespace Warehouse.ClientAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ISender _sender;

        public OrderController(ISender sender)
        {
            _sender = sender;
        }


        [HttpPost]
        public async Task<IActionResult> MakeOrder([FromBody] MakeOrderRequest request)
        {
            var command = new MakeOrderCommand() { ProductId = request.ProductId };

             await _sender.Send(command);

            return Ok();
        }
    }
}
