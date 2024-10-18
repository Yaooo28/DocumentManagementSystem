using Microsoft.VisualBasic.FileIO;
using System;

namespace DocumentManagementSystem.Entities
{
    public class Document : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string TypeOfDoc { get; set; }
        public string ClassOfDoc { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public DocState DocState { get; set; }
        public DocStatus DocStatus { get; set; } = DocStatus.Submitted;
        public int ReplyDocId { get; set; }
        public DateTime SendDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ReceiveDate { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string ShelfNumber { get; set; } = string.Empty;
        public bool isBorrowed { get; set; } = false;
        public string BorrowerName { get; set; } = string.Empty;
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public int? DepId { get; set; }
        public string FileName { get; set; }  // Add this field for file name
        public string FilePath { get; set; }  // Add this field for file path
    }

    public enum DocState
    {
        Physical,
        Electronic
    }
    public enum DocStatus
    {
        Submitted,
        Ongoing,
        Recieved,
        Forwarded,
        Done
    }

}