using System;
using System.Collections.Generic;
using System.Text;
using Dentigo.Domain.Enums;
namespace Dentigo.Domain.Entities
{
    public class CaseFile
    {
        public int Id { get; set; }
        public int PatientId { get; set; } //fk1
        public int DentalCaseId { get; set; }//fk2
        public int? ToothId { get; set; } //fk3 not needed at all cases
        public int UploadedByUserId { get; set; } //fk4
        public string FileName { get; set; } = null!;
        public string FileType { get; set; } = null!;
        public long FileSize { get; set; }
        public string StoragePath { get; set; } = null!;
        public FileCategory FileCategory { get; set; }
        public DateTime UploadedAt { get; set; }
        public Patient Patient { get; set; } = null!;
        public DentalCase DentalCase { get; set; } = null!;
        public Tooth? Tooth { get; set; }
        public User UploadedByUser { get; set; } = null!;

    }
}
