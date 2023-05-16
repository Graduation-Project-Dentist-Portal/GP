using DentistPortal_API.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentistPortal_Client.Model
{
    public class PatientCase
    {
        public Guid Id { get; set; }

        public string PatientPhone { get; set; } = string.Empty;
        public int PatientAge { get; set; }
        public string Description { get; set; } = string.Empty;
        public Guid PatientId { get; set; }
        public Guid? AssignedDoctorId { get; set; }
        public bool IsActive { get; set; }
        public string CaseStatus { get; set; } = string.Empty;
        public DateTime TimeCreated { get; set; }
        [ForeignKey("PatientId")]
        public Patient Patient { get; set; } = new();
    }
}
