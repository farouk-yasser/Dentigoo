using System;
using System.Collections.Generic;
using System.Text;
using Dentigo.Domain.Enums;

namespace Dentigo.Domain.Entities
{
    public class DentalCase
    {
        public int Id { get; set; }

        public int PatientId { get; set; }//fk1

        public int ToothId { get; set; }//fk2

        public int CreatedByUserId { get; set; }//fk3

        public string TreatmentName { get; set; } = null!;

        public string? Description { get; set; }

        public string? DoctorInstructions { get; set; }

        public DentalCaseStatus Status { get; set; } //Enum 

        public bool RequiresRadiology { get; set; }

        public DateTime? ExpectedDueDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? CancellationReason { get; set; }

        // Navigation Properties
        public Patient Patient { get; set; } = null!;

        public Tooth Tooth { get; set; } = null!;

        public User CreatedByUser { get; set; } = null!;
    }

}

