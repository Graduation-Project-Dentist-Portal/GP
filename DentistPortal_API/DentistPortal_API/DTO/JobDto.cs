namespace DentistPortal_API.DTO
{
    public class JobDto
    {
        public string JobTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Salary { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string? ContactNumber { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public Guid OwnerIdDoctor { get; set; }
    }
}
