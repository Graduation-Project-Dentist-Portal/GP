namespace DentistPortal_API.Model
{
    public class Like
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid FeedbackId { get; set; }
        public bool IsActive { get; set; }
    }
}
