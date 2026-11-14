namespace Dentigo.Presentation.Models.Patient
{
    public class PatientViewModel 
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}