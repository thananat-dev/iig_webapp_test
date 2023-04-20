using AutoMapper;
using iig_webapp_test.Entities;
using iig_webapp_test.Models;

namespace iig_webapp_test.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // RegisterRequest -> User
            CreateMap<RegisterRequest, User>();

            // UpdateProfileRequest -> User
            CreateMap<UpdateProfileRequest, User>();
        }
    }
}
