using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.AdminAPI.ApiModels;
using Warehouse.AdminAPI.Application.Queires.Categories;

namespace Warehouse.AdminAPI.Controllers
{
    [Route("categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ISender _sender;

        public CategoryController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet()]
        public async Task<IActionResult> Categories()
        {
            var response = await _sender.Send(new GetCategoriesQuery());

            var categories = response.Categories.Select(x => new
            {
                Id = x.Id,
                Name = x.Name,
            });

            return Ok(categories);

        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryRequest request)
        {
            var response = await _sender.Send(new GetCategoriesQuery());

            var categories = response.Categories.Select(x => new
            {
                Id = x.Id,
                Name = x.Name,
            });

            return Ok(categories);

        }

        [HttpPut]
        public async Task<IActionResult> Update()
        {
            var response = await _sender.Send(new GetCategoriesQuery());

            var categories = response.Categories.Select(x => new
            {
                Id = x.Id,
                Name = x.Name,
            });

            return Ok(categories);

        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            var response = await _sender.Send(new GetCategoriesQuery());

            var categories = response.Categories.Select(x => new
            {
                Id = x.Id,
                Name = x.Name,
            });

            return Ok(categories);

        }
    }
}