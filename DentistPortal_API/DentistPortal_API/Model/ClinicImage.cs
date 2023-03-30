namespace DentistPortal_API.Model
{
    public class ClinicImage
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public Guid ClinicId { get; set; }
        public bool IsActive { get; set; }
    }
}
