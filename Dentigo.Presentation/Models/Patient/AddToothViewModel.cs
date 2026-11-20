using System.ComponentModel.DataAnnotations;

namespace Dentigo.Presentation.Models.Patient
{
    public class AddToothViewModel
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid patient ID.")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Tooth number is required.")]
        [StringLength(10, ErrorMessage = "Tooth number cannot exceed 10 characters.")]
        public string ToothNumber { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }
    }
}
