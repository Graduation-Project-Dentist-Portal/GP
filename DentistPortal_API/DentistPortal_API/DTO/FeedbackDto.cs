namespace DentistPortal_API.DTO
{
    public class FeedbackDto
    {
        public string Comment { get; set; } = string.Empty;
        public Guid ClinicId { get; set; }
        public Guid UserId { get; set; }
    }
}
