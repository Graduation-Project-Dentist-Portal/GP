using FluentMigrator;
using Microsoft.AspNetCore.Identity;

namespace DentistPortal_API.Migrations
{
    [Migration(12)]
    public class _0012_SeedDentist : Migration
    {
        record dentist
        {
            public Guid Id { get; set; }
            public string Username { get; set; } = String.Empty;
            public string FirstName { get; set; } = String.Empty;
            public string LastName { get; set; } = String.Empty;
            public string PasswordHash { get; set; } = String.Empty;
            public Guid RefreshTokenId { get; set; }
            public string ProfilePicture { get; set; } = String.Empty;
            public bool IsActive { get; set; }
            public bool Graduated { get; set; }
            public string University { get; set; } = String.Empty;
            public string IdentityCardPicture { get; set; } = String.Empty;
            public string UniversityCardPicture { get; set; } = String.Empty;
            public int Level { get; set; }
        }
        static PasswordHasher<dentist> hasher = new();

        public override void Down()
        {
        }

        public override void Up()
        {
            Insert.IntoTable(tableName: "Dentist").Row(new
            {
                Id = Guid.Parse("0b89bb77-33ef-471d-8390-59b3969d86ae"),
                Username = "YounesAbady",
                FirstName = "Younes",
                LastName = "Abady",
                PasswordHash = hasher.HashPassword(new dentist(), "123"),
                IsActive = true,
                Graduated = true,
                University = "Helwan",
                IdentityCardPicture = "test",
                UniversityCardPicture = "test",
                Level = 0
            });
        }
    }
}
