namespace DentistPortal_Client.DTO
{
    public class ClinicDto
    {
        public string Address { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ClinicDescription { get; set; } = string.Empty;
        public Guid DoctorId { get; set; }
        public string ClinicPhone { get; set; } = string.Empty;
        public DateTime OpenTime { get; set; }
        public DateTime CloseTime { get; set; }
        public List<IFormFile> CasePictures { get; set; } = new();
    }
}
