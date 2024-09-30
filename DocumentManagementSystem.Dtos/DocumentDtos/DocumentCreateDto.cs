using DocumentManagementSystem.Dtos.Interfaces;
using DocumentManagementSystem.Entities;



namespace DocumentManagementSystem.Dtos.DocumentDtos
{
    public class DocumentCreateDto : IDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string TypeOfDoc { get; set; }
        public string ClassOfDoc { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public DocState DocState { get; set; }
        public int AppUserId { get; set; }
        public int DepId { get; set; }
    }
}
