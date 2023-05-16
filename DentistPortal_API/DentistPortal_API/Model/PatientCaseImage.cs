namespace DentistPortal_API.Model
{
    public class PatientCaseImage
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public Guid PatientCaseId { get; set; }
        public bool IsActive { get; set; }
    }
}
