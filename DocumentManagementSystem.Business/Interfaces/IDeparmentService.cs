using DocumentManagementSystem.Dtos;
using DocumentManagementSystem.Entities;

namespace DocumentManagementSystem.Business.Interfaces
{
    public interface IDeparmentService : IService<DepartmentCreateDto, DepartmentUpdateDto, DepartmentListDto, Department>
    {
    }
}
