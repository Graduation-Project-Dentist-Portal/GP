namespace DentistPortal_Client.DTO
{
    public class MedicalCaseDto
    {
        public string Description { get; set; } = String.Empty;
        public string PatientName { get; set; } = String.Empty;
        public string PatientPhone { get; set; } = String.Empty;
        public int PatientAge { get; set; }
        public string Diagnosis { get; set; } = String.Empty;
        public Guid DoctorId { get; set; }
        public List<string> CasePictures { get; set; } = new();
        public bool AssignedToMe { get; set; }
    }
}
