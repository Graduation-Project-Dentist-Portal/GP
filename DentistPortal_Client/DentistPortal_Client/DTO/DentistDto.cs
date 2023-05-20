namespace DentistPortal_Client.DTO
{
    public class DentistDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = String.Empty;
        public string FirstName { get; set; } = String.Empty;
        public string LastName { get; set; } = String.Empty;
        public string PasswordHash { get; set; } = String.Empty;
        public Guid? RefreshTokenId { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        public bool IsActive { get; set; }
        public bool Graduated { get; set; }
        public string University { get; set; } = String.Empty;
        public IFormFile? IdentityCardPicture { get; set; }
        public IFormFile? UniversityCardPicture { get; set; }
        public int Level { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
