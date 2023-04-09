namespace DentistPortal_API.Model
{
    public class Dentist
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public Guid? RefreshTokenId { get; set; }
        public string? ProfilePicture { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool Graduated { get; set; }
        public string University { get; set; } = string.Empty;
        public string IdentityCardPicture { get; set; } = string.Empty;
        public string UniversityCardPicture { get; set; } = string.Empty;
        public int Level { get; set; }
        public string IsVerified { get; set; } = string.Empty;
        public string? VerfiyMessage { get; set; } = string.Empty;

    }
}
