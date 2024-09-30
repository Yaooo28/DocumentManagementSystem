using System.Collections.Generic;

namespace DocumentManagementSystem.Entities
{
    public class Department : BaseEntity
    {
        public string Definition { get; set; }
        public List<AppUser> AppUsers { get; set; }

    }
}
