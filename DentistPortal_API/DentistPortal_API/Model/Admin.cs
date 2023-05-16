namespace DentistPortal_API.Model
{
    public class Admin
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public Guid? RefreshTokenId { get; set; }
        public bool IsActive { get; set; }
    }
}
