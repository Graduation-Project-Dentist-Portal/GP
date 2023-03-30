namespace DentistPortal_Client.DTO
{
    public class ToolDto
    {
        public string ToolName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double ToolPrice { get; set; }
        public string SellerLocation { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public Guid SellerIdDoctor { get; set; }
        public List<IFormFile> Pictures { get; set; } = new();
        public string ToolStatus { get; set; } = string.Empty;
    }
}
