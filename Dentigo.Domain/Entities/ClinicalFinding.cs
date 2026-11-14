using System;
using System.Collections.Generic;
using System.Text;

namespace Dentigo.Domain.Entities
{
   public class ClinicalFinding 
    {
        public int Id { get; set; }
        public int PatientId { get; set; }//fk1
        public int ToothId { get; set; }//fk2
        public int? DentalCaseId { get; set;}//fk3
        public string Description { get; set; } = null!; 
        public int CreatedByUserId { get; set; }//fk4
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Patient Patient { get; set; } = null!;
        public Tooth Tooth { get; set; } = null!;
        public DentalCase? DentalCase { get; set; }
        public User CreatedByUser { get; set; } = null!;

    }
}
