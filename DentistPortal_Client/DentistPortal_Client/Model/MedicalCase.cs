namespace DentistPortal_API.Model
{
    public class MedicalCase
    {
        public Guid Id { get; set; }
        public Guid? AssignedDoctorId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string PatientPhone { get; set; } = string.Empty;
        public int PatientAge { get; set; } 
        public Guid DoctorId { get; set; }
        public bool IsActive { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string CaseStatus { get; set; } = string.Empty;
        public DateTime TimeCreated { get; set; }
    }
}
