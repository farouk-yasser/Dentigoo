namespace Dentigo.Presentation.Models.Patient
{
    public class DentalChartViewModel
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; } = null!;

        public List<ToothViewModel> Teeth { get; set; } = new();
    }

    public class ToothViewModel
    {
        public int Id { get; set; }
        public string ToothNumber { get; set; } = null!;
        public string? Notes { get; set; }
    }
}