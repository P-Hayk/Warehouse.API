using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.AdminAPI.ApiModels.Categories;
using Warehouse.AdminAPI.Application.Commands.Categories;
using Warehouse.AdminAPI.Application.Queires.Categories;
using Warehouse.Application.Commands;

namespace Warehouse.AdminAPI.Controllers
{
    [Route("categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public CategoryController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var response = await _sender.Send(new GetCategoriesQuery());

            var categories = _mapper.Map<ICollection<CategoryApiModel>>(response.Categories);

            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryRequest request)
        {
            var command = _mapper.Map<CreateCategoryCommand>(request);
            await _sender.Send(command);

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update()
        {
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok();
        }
    }
}