namespace DentistPortal_Client.DTO
{
    public class FullUserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = String.Empty;
        public string FirstName { get; set; } = String.Empty;
        public string LastName { get; set; } = String.Empty;
        public string? ProfilePicture { get; set; } = String.Empty;
        public bool Graduated { get; set; }
        public string University { get; set; } = String.Empty;
        public int Level { get; set; }
    }
}
