using DocumentManagementSystem.Dtos.Interfaces;

namespace DocumentManagementSystem.Dtos
{
    public class DepartmentListDto : IDto
    {
        public int Id { get; set; }
        public string Definition { get; set; }
    }
}
