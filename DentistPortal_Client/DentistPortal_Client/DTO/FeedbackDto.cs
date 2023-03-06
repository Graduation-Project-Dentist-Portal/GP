namespace DentistPortal_Client.DTO
{
    public class FeedbackDto
    {
        public string Comment { get; set; } = string.Empty;
        public Guid ClinicId { get; set; }
        public Guid UserId { get; set; }
    }
}
