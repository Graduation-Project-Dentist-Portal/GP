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
            public string Username { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string PasswordHash { get; set; } = string.Empty;
            public Guid RefreshTokenId { get; set; }
            public string ProfilePicture { get; set; } = string.Empty;
            public bool IsActive { get; set; }
            public bool Graduated { get; set; }
            public string University { get; set; } = string.Empty;
            public string IdentityCardPicture { get; set; } = string.Empty;
            public string UniversityCardPicture { get; set; } = string.Empty;
            public int Level { get; set; }
            public string IsVerified { get; set; } = string.Empty;
            public string? VerfiyMessage { get; set; } = string.Empty;
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
                Level = 0,
                IsVerified = "true",
                VerfiyMessage = ""
            })
            .Row(new
            {
                Id = Guid.Parse("e0406609-ed2e-466d-87b8-7b8fb1c34800"),
                Username = "MohamedKhaled",
                FirstName = "Mohamed",
                LastName = "Khaled",
                PasswordHash = hasher.HashPassword(new dentist(), "123"),
                IsActive = true,
                Graduated = false,
                University = "Helwan",
                IdentityCardPicture = "test",
                UniversityCardPicture = "test",
                Level = 3,
                IsVerified = "true",
                VerfiyMessage = ""
            });
        }
    }
}
