namespace DentistPortal_API.DTO
{
    public class MedicalCaseDto
    {
        public string Description { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string PatientPhone { get; set; } = string.Empty;
        public int PatientAge { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public Guid DoctorId { get; set; }
        public List<IFormFile> CasePictures { get; set; } = new();
        public bool AssignedToMe { get; set; }
    }
}
