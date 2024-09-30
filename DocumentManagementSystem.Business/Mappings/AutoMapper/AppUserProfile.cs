using AutoMapper;
using DocumentManagementSystem.Dtos;
using DocumentManagementSystem.Entities;

namespace DocumentManagementSystem.Business.Mappings.AutoMapper
{
    public class AppUserProfile : Profile
    {
        public AppUserProfile()
        {
            CreateMap<AppUser, AppUserCreateDto>().ReverseMap();
            CreateMap<AppUser, AppUserUpdateDto>().ReverseMap();
            CreateMap<AppUser, AppUserListDto>().ReverseMap();

        }
    }
}