using AutoMapper;
using Warehouse.AdminAPI.ApiModels.Categories;
using Warehouse.Domain.Models;

namespace Warehouse.AdminAPI.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryApiModel>();
        }
    }
}