using System.ComponentModel.DataAnnotations;

namespace Dentigo.Presentation.Models.Patient
{
    public class CreateClinicalFindingViewModel
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid patient ID.")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Please select a tooth.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid tooth.")]
        public int? ToothId { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; } = null!;
    }
}
