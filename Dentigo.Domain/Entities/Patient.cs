using System;
using System.Collections.Generic;
using System.Text;

namespace Dentigo.Domain.Entities
{
    public class Patient
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
