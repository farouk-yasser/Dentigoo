using System;
using System.Collections.Generic;
using System.Text;

namespace Dentigo.Domain.Entities
{
    // (Case timeline)
    public class CaseHistoryEvent 
    {
        public int Id { get; set; }
        public int DentalCaseId { get; set; }//fk
        public int? ActorUserId { get; set; }//fk2
        public string EventType { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? PreviousStatus { get; set;}
        public string? NewStatus { get; set; }
        public int? RelatedFileId { get; set; }
        public int? RelatedDesignVersionId { get; set; }
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DentalCase DentalCase { get; set; } = null!;
        public User? ActorUser { get; set; }
        public CaseFile? RelatedFile { get; set; }
        public DesignVersion? RelatedDesignVersion { get; set; }

    }
}
