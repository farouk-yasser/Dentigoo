using System.ComponentModel.DataAnnotations;

namespace Dentigo.Presentation.Models.Patient
{
    public class EditPatientViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(150)]
        public string FullName { get; set; } = null!;

        [StringLength(20)]
        public string? PhoneNumber { get; set; }
    }
}