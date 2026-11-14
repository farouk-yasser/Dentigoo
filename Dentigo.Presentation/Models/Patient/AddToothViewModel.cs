using System.ComponentModel.DataAnnotations;

namespace Dentigo.Presentation.Models.Patient
{
    public class AddToothViewModel
    {
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Tooth number is required")]
        [StringLength(10)]
        public string ToothNumber { get; set; } = null!;

        public string? Notes { get; set; }
    }
}