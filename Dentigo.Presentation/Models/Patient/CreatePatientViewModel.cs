using System.ComponentModel.DataAnnotations;

namespace Dentigo.Presentation.Models.Patient
{
    public class CreatePatientViewModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(150, ErrorMessage = "Full name cannot exceed 150 characters.")]
        public string FullName { get; set; } = null!;

        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string? PhoneNumber { get; set; }
    }
}
