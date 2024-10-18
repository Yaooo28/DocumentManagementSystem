using DocumentManagementSystem.Dtos.Interfaces;
using DocumentManagementSystem.Entities;

namespace DocumentManagementSystem.Dtos
{
    public class DocumentRemarksDto : IUpdateDto
    {
        public int Id { get; set; }
        public string Title { get; set; } // Added Title property
        public string Description { get; set; }
        public DocStatus DocStatus { get; set; }
        public int DepId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }

}
