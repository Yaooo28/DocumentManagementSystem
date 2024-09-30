using DocumentManagementSystem.Dtos.Interfaces;

namespace DocumentManagementSystem.Dtos
{
    public class AppRoleListDto : IDto
    {
        public int Id { get; set; }
        public string Definition { get; set; }
    }
}
