namespace DentistPortal_Client.Model
{
    public class Clinic
    {
        public Guid Id { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ClinicDescription { get; set; } = string.Empty;
        public Guid DoctorId { get; set; }
        public string ClinicPhone { get; set; } = string.Empty;
        public int OpenTime { get; set; }
        public int CloseTime { get; set; }
        public string PicturePaths { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
