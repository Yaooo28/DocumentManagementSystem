using AutoMapper;
using DocumentManagementSystem.Business.Interfaces;
using DocumentManagementSystem.DataAccess.UnitOfWork;
using DocumentManagementSystem.Dtos;
using DocumentManagementSystem.Dtos.DocumentDtos;
using DocumentManagementSystem.Entities;
using FluentValidation;

namespace DocumentManagementSystem.Business.Services
{
    public class DocumentService : Service<DocumentCreateDto, DocumentUpdateDto, DocumentListDto, Document>, IDocumentService
    {
        public DocumentService(IMapper mapper, IValidator<DocumentCreateDto> createDtoValidator, IValidator<DocumentUpdateDto> updateDtoValidator, IUow uow) : base(mapper, createDtoValidator, updateDtoValidator, uow)
        {

        }
    }
}
