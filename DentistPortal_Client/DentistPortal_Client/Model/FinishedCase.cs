namespace DentistPortal_Client.Model
{
    public class FinishedCase
    {
        public Guid DoctorId { get; set; }
        public string DoctorWork { get; set; } = string.Empty;
        public string BeforePicture { get; set; }
        public string AfterPicture { get; set; }
    }
}
