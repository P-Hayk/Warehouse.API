using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Commands;
using Warehouse.ClientAPI.ApiModels;
using Warehouse.Domain.Models;
using Warehouse.Infrastructure.Extensions;

namespace Warehouse.ClientAPI.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrderController : ControllerBase
    {
        private readonly ISender _sender;

        private readonly IMapper _mapper;
        public OrderController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MakeOrder([FromBody] MakeOrderRequest request)
        {
            var clientId = HttpContext.GetClientId();

            var command = _mapper.Map<MakeOrderCommand>(request);

            command.ClientId = clientId;

            await _sender.Send(command);
            return Ok();
        }
    }
}
