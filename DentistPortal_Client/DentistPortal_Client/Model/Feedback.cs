using DentistPortal_API.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentistPortal_Client.Model
{
    public class Feedback
    {
        public Guid Id { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string AiScore { get; set; } = string.Empty;
        public Guid ClinicId { get; set; }
        public Guid UserId { get; set; }
        public bool IsActive { get; set; }
        public int Likes { get; set; }
        [ForeignKey("UserId")]
        public Patient Patient { get; set; } = new();
    }
}
