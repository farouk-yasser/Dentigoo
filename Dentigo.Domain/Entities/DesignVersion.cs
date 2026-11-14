using System;
using System.Collections.Generic;
using System.Text;
using Dentigo.Domain.Enums;
namespace Dentigo.Domain.Entities
{
    public class DesignVersion
    {
        public int Id { get; set;}
        public int DentalCaseId { get; set;} //fk1
        public int CaseFileId { get; set;}//fk2
        public int SubmittedByUserId { get; set;} //fkk3
        public int VersionNumber { get; set; }
        public DesignReviewStatus ReviewStatus { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string? ReviewReason { get; set; } 
       public DentalCase DentalCase { get; set; } = null!;
       public CaseFile CaseFile { get; set; } = null!;
       public User SubmittedByUser { get; set; } = null!;
    }
}
