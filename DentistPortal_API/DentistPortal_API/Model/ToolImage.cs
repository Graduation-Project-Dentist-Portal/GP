namespace DentistPortal_API.Model
{
    public class ToolImage
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public Guid ToolId { get; set; }
        public bool IsActive { get; set; }
    }
}
