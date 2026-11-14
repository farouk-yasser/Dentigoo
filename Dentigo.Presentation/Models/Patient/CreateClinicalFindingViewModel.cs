
using System.ComponentModel.DataAnnotations;

namespace Dentigo.Presentation.Models.Patient
{
    public class CreateClinicalFindingViewModel
    {
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Please select a tooth")]
        public int ToothId { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000)]
        public string Description { get; set; } = null!;
    }
}
