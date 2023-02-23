namespace DentistPortal_API.Model
{
    public class Picture
    {
        public Guid Id { get; set; }
        public string Path { get; set; }
        public Guid OwnerId { get; set; }
        public bool IsActive { get; set; }
    }
}
