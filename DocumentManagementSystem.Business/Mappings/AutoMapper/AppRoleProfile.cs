using AutoMapper;
using DocumentManagementSystem.Dtos;
using DocumentManagementSystem.Entities;

namespace DocumentManagementSystem.Business.Mappings.AutoMapper
{
    public class AppRoleProfile : Profile
    {
        public AppRoleProfile()
        {
            CreateMap<AppRole, AppRoleListDto>().ReverseMap();
        }
    }
}
