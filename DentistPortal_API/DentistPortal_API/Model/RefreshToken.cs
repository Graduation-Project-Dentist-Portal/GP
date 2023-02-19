namespace DentistPortal_API.Model
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime TimeCreated { get; set; }
        public DateTime TimeExpires { get; set; }
        public bool IsActive { get; set; }
    }
}
