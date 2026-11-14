using System;
using System.Collections.Generic;
using System.Text;
using Dentigo.Domain.Enums;
namespace Dentigo.Domain.Entities
{
    public class TreatmentPlan
    {
        public int Id {get; set;}
        public string Name { get; set; } = null!;  
        public string? Description { get; set;}
        public int CreatedByUserId { get; set;}
        public DateTime CreatedAt { get; set; }
        public Patient Patient { get; set; } = null!;
        public int PatientId { get; set; }
        public User CreatedByUser { get; set; } = null!;
        public decimal AgreedCost { get; set; }
        public TreatmentPlanStatus treatmentPlanStatus { get; set; } = TreatmentPlanStatus.Open;
  
    }
}
