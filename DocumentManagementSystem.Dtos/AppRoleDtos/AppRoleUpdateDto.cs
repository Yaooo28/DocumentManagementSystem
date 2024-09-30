using DocumentManagementSystem.Dtos.Interfaces;

namespace DocumentManagementSystem.Dtos
{
    public class AppRoleUpdateDto : IUpdateDto
    {
        public int Id { get; set; }
        public string Definition { get; set; }
    }
}
