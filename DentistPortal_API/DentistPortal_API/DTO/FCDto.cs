namespace DentistPortal_API.DTO
{
    public class FCDto
    {
        public Guid DoctorId { get; set; }
        public string DoctorWork { get; set; } = string.Empty;
        public string BeforePicture { get; set; }
        public string AfterPicture { get; set; }
    }
}
