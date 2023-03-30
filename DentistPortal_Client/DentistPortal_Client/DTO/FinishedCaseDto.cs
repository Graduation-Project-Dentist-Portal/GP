namespace DentistPortal_Client.DTO
{
    public class FinishedCaseDto
    {
        public Guid DoctorId { get; set; }
        public Guid CaseId { get; set; }
        public string DoctorWork { get; set; } = string.Empty;
        public IFormFile BeforePicture { get; set; }
        public IFormFile AfterPicture { get; set; }
    }
}
