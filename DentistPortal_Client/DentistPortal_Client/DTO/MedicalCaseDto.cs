namespace DentistPortal_API.DTO
{
    public class MedicalCaseDto
    {
        public string Description { get; set; } = String.Empty;
        public string PatientName { get; set; } = String.Empty;
        public string PatientPhone { get; set; } = String.Empty;
        public string PatientAge { get; set; } = String.Empty;
        public Guid DoctorId { get; set; }
    }
}
