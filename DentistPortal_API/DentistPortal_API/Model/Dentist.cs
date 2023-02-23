namespace DentistPortal_API.Model
{
    public class Dentist
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = String.Empty;
        public string FirstName { get; set; } = String.Empty;
        public string LastName { get; set; } = String.Empty;
        public string PasswordHash { get; set; } = String.Empty;
        public Guid? RefreshTokenId { get; set; }
        public string? ProfilePicture { get; set; } = String.Empty;
        public bool IsActive { get; set; }
        public bool Graduated { get; set; }
        public string University { get; set; } = String.Empty;
        public string IdentityCardPicture { get; set; } = String.Empty;
        public string UniversityCardPicture { get; set; } = String.Empty;
        public int Level { get; set; }
    }
}
