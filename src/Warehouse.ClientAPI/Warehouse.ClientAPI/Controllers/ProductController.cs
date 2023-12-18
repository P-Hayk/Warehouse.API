using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Queries;
using Warehouse.ClientAPI.ApiModels;

namespace Warehouse.ClientAPI.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public ProductController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        [HttpGet("products")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<ProductApiModel>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Products()
        {
            var response = await _sender.Send(new GetProductsQuery());
            var products = _mapper.Map<ProductApiModel>(response);

            return Ok(products);
        }
    }
}
