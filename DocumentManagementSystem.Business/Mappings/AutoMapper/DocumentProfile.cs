using AutoMapper;
using DocumentManagementSystem.Dtos;
using DocumentManagementSystem.Dtos.DocumentDtos;
using DocumentManagementSystem.Entities;

namespace DocumentManagementSystem.Business.Mappings.AutoMapper
{
    public class DocumentProfile : Profile
    {
        public DocumentProfile()
        {
            CreateMap<Document, DocumentListDto>().ReverseMap();
            CreateMap<Document, DocumentCreateDto>().ReverseMap();
            CreateMap<Document, DocumentUpdateDto>().ReverseMap();
            CreateMap<Document, DocumentRemarksDto>().ReverseMap();
        }
    }
}
