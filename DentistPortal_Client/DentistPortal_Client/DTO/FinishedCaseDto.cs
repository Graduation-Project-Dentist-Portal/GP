namespace DentistPortal_Client.DTO
{
    public class FinishedCaseDto
    {
        public Guid DoctorId { get; set; }
        public Guid CaseId { get; set; }
        public string DoctorWork { get; set; } = string.Empty;
        public string BeforePicture { get; set; } = string.Empty;
        public string AfterPicture { get; set; } = string.Empty;
    }
}
