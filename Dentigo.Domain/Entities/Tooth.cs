using System;
using System.Collections.Generic;
using System.Text;

namespace Dentigo.Domain.Entities
{
    public class Tooth
    {
        public int Id { get; set;} 
        public int PatientId { get; set; } //Fk each tooth related to patient (required)
        public string ToothNumber { get; set; } = null!;
        public string? Notes { get; set; }
        public Patient Patient { get; set; } = null!;
    }

}

