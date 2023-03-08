namespace DentistPortal_Client.DTO
{
    public class ClinicDto
    {
        public string Address { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ClinicDescription { get; set; } = string.Empty;
        public Guid DoctorId { get; set; }
        public string ClinicPhone { get; set; } = string.Empty;
        //public int OpenTime { get; set; }
        public DateTime OpenTime { get; set; }
        public DateTime CloseTime { get; set; }
        //public int CloseTime { get; set; }
        public List<string> CasePictures { get; set; } = new();
    }
}
