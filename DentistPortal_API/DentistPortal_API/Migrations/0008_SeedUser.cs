using FluentMigrator;
using Microsoft.AspNetCore.Identity;

namespace DentistPortal_API.Migrations
{
    [Migration(8)]
    public class _0008_SeedUser : Migration
    {
        record user
        {
            public Guid Id { get; set; }
            public string Username { get; set; } = String.Empty;
            public string FirstName { get; set; } = String.Empty;
            public string LastName { get; set; } = String.Empty;
            public string PasswordHash { get; set; } = String.Empty;
            public Guid RefreshTokenId { get; set; }
            public string ProfilePicture { get; set; } = String.Empty;
            public string Role { get; set; } = String.Empty;
            public bool IsActive { get; set; }
        }
        static PasswordHasher<user> hasher = new();
        public override void Down()
        {
        }

        public override void Up()
        {
            Insert.IntoTable(tableName: "User").Row(new
            {
                Id = Guid.Parse("0b89bb77-33ef-471d-8390-59b3969d86ae"),
                Username = "YounesAbady",
                FirstName = "Younes",
                LastName = "Abady",
                PasswordHash = hasher.HashPassword(new user(), "123"),
                Role = "Dentist",
                IsActive = true
            }).Row(new
            {
                Id = Guid.Parse("63c108fc-aaf8-4f5b-a795-7beb3f29d76a"),
                Username = "HagarMohamed",
                FirstName = "Hagar",
                LastName = "Mohamed",
                PasswordHash = hasher.HashPassword(new user(), "123"),
                Role = "Student",
                IsActive = true
            }).Row(new
            {
                Id = Guid.Parse("1cc48874-ac1c-468d-b25b-d6af0425b86d"),
                Username = "MohamedKhaled",
                FirstName = "Mohamed",
                LastName = "Khaled",
                PasswordHash = hasher.HashPassword(new user(), "123"),
                Role = "Patient",
                IsActive = true
            });
        }
    }
}
