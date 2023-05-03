namespace DentistPortal_API.Model
{
    public class Job
    {
        public Guid Id { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Salary { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public Guid OwnerIdDoctor { get; set; }
        public bool IsActive { get; set; }
    }
}
