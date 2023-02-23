namespace DentistPortal_API.Model
{
    public class MedicalCase
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = String.Empty;
        public string PatientName { get; set; } = String.Empty;
        public string PatientPhone { get; set; } = String.Empty;
        public string PatientAge { get; set; } = String.Empty;
        public Guid DoctorId { get; set; }
        public bool IsActive { get; set; }
        public string PicturePaths { get; set; } = String.Empty;
        public string Diagnosis { get; set; } = String.Empty;
    }
}
