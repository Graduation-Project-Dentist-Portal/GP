using FluentMigrator;
using Microsoft.AspNetCore.Identity;

namespace DentistPortal_API.Migrations
{
    [Migration(9)]
    public class _0009_SeedPatient : Migration
    {
        record patient
        {
            public Guid Id { get; set; }
            public string Username { get; set; } = String.Empty;
            public string FirstName { get; set; } = String.Empty;
            public string LastName { get; set; } = String.Empty;
            public string PasswordHash { get; set; } = String.Empty;
            public Guid RefreshTokenId { get; set; }
            public string ProfilePicture { get; set; } = String.Empty;
            public bool IsActive { get; set; }
        }
        static PasswordHasher<patient> hasher = new();
        public override void Down()
        {
        }

        public override void Up()
        {
            Insert.IntoTable(tableName: "Patient").Row(new
            {
                Id = Guid.Parse("63c108fc-aaf8-4f5b-a795-7beb3f29d76a"),
                Username = "HagarMohamed",
                FirstName = "Hagar",
                LastName = "Mohamed",
                PasswordHash = hasher.HashPassword(new patient(), "123"),
                IsActive = true
            });
        }
    }
}
