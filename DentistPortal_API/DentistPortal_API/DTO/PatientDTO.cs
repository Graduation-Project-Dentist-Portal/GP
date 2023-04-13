namespace DentistPortal_API.DTO
{
    public class PatientDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = String.Empty;
        public string FirstName { get; set; } = String.Empty;
        public string LastName { get; set; } = String.Empty;
        public string PasswordHash { get; set; } = String.Empty;
        public Guid? RefreshTokenId { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        public bool IsActive { get; set; }
    }
}
