namespace DentistPortal_API.Model
{
    public class Feedback
    {
        public Guid Id { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string AiScore { get; set; } = string.Empty;
        public Guid ClinicId { get; set; }
        public Guid UserId { get; set; }
        public bool IsActive { get; set; }  
    }
}
