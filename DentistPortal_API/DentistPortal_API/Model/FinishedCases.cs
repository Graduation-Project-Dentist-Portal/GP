namespace DentistPortal_API.Model
{
    public class FinishedCases
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public Guid CaseId { get; set; }
        public string DoctorWork { get; set; } = String.Empty;
        public string BeforePicture { get; set; } = String.Empty;
        public string AfterPicture { get; set; } = String.Empty;
    }
}
