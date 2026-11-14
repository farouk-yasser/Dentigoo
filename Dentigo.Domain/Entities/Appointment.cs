using System;
using System.Collections.Generic;
using System.Text;
using Dentigo.Domain.Enums;

namespace Dentigo.Domain.Entities
{
    public class Appointment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly AppointmentTime { get; set; }
        public VisitReason VisitReason { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? Notes { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Patient Patient { get; set; } = null!;
        public User Doctor { get; set; } = null!;
        public User CreatedByUser { get; set; } = null!;
    }
}
