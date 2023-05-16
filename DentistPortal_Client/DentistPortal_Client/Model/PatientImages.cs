namespace DentistPortal_Client.Model
{
    public class PatientImages
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public Guid PatientCaseId { get; set; }
        public bool IsActive { get; set; }
    }
}
