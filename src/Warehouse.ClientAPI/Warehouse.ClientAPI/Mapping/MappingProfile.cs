using AutoMapper;
using Warehouse.Application.Commands;
using Warehouse.ClientAPI.ApiModels;

namespace Warehouse.ClientAPI.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MakeOrderRequest, MakeOrderCommand>();
        }
    }
}