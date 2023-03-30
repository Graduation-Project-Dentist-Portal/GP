namespace DentistPortal_API.Model
{
    public class Clinic
    {
        public Guid Id { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ClinicDescription { get; set; } = string.Empty;
        public Guid DoctorId { get; set; }
        public string ClinicPhone { get; set; } = string.Empty;
        public DateTime OpenTime { get; set; }
        public DateTime CloseTime { get; set; }
        public bool IsActive { get; set; }
    }
}
