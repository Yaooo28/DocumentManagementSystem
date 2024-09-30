using AutoMapper;
using DocumentManagementSystem.Dtos;
using DocumentManagementSystem.Entities;

namespace DocumentManagementSystem.Business.Mappings.AutoMapper
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<Department, DepartmentCreateDto>().ReverseMap();
            CreateMap<Department, DepartmentUpdateDto>().ReverseMap();
            CreateMap<Department, DepartmentListDto>().ReverseMap();

        }
    }
}
