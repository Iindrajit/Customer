using AutoMapper;
using CustomerAPI.Entities;
using CustomerAPI.Models;

namespace CustomerAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Customer, CustomerDto>();
            CreateMap<CreateCustomerDto, Customer>();
            CreateMap<UpdateCustomerDto, Customer>().ReverseMap();
        }
    }
}
