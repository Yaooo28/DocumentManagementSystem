using DocumentManagementSystem.Dtos.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Dtos
{
    public class DepartmentCreateDto : IDto
    {
        public string Definition { get; set; }
        public string Title { get; set; }
        public string TypeOfDoc { get; set; }
        public string ClassOfDoc { get; set; }
        public int DocState { get; set; }
        public string Description { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string Department { get; set; }
    }
}
