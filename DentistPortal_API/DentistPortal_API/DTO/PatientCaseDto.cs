namespace DentistPortal_API.DTO
{
    public class PatientCaseDto
    {
        public string Description { get; set; } = string.Empty;
        public string PatientPhone { get; set; } = string.Empty;
        public int PatientAge { get; set; }
        public Guid PatientId { get; set; }
        public List<IFormFile> PatientCasePictures { get; set; } = new();
        public Guid AssignedDoctorId { get; set; }

    }
}
