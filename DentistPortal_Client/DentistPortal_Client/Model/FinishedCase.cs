using DentistPortal_API.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentistPortal_Client.Model
{
    public class FinishedCase
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public Guid CaseId { get; set; }
        public string DoctorWork { get; set; } = string.Empty;
        public string BeforePicture { get; set; } = string.Empty;
        public string AfterPicture { get; set; } = string.Empty;
        public Guid? MedicalCaseId { get; set; }
        public Guid? PatientCaseId { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("MedicalCaseId")]
        public virtual MedicalCase MedicalCase { get; set; }

        [ForeignKey("PatientCaseId")]
        public virtual PatientCase PatientCase { get; set; }

    }
}
