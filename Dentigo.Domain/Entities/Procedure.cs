using System;
using System.Collections.Generic;
using System.Text;

namespace Dentigo.Domain.Entities
{
    public class Procedure
    {
        public int Id { get; set; }
        public int TreatmentPlanId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int SequenceNumber { get; set; }
        public int? DentalCaseId { get; set; }
        
        public DentalCase? DentalCase { get; set; }
        public TreatmentPlan TreatmentPlan { get; set; } = null!;

    }
}
