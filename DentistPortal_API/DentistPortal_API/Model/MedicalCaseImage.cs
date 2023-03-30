namespace DentistPortal_API.Model
{
    public class MedicalCaseImage
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public Guid MedicalCaseId { get; set; }
        public bool IsActive { get; set; }
    }
}
