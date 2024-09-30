using DocumentManagementSystem.Dtos;
using DocumentManagementSystem.Dtos.DocumentDtos;
using DocumentManagementSystem.Entities;

namespace DocumentManagementSystem.Business.Interfaces
{
    public interface IDocumentService : IService<DocumentCreateDto, DocumentUpdateDto, DocumentListDto, Document>
    {
    }
}
