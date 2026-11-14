using System;
using System.Collections.Generic;
using System.Text;

namespace Dentigo.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int? DentalCaseId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public int RecordedByUserId { get; set; }
        public Patient Patient { get; set; } = null!;
        public DentalCase? DentalCase { get; set; }
        public User RecordedByUser { get; set; } = null!;
        public int? TreatmentPlanId { get; set; }
       public TreatmentPlan? TreatmentPlan { get; set; }
    }
}
