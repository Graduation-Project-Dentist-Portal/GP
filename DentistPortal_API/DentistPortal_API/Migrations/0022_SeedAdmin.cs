using FluentMigrator;
using Microsoft.AspNetCore.Identity;

namespace DentistPortal_API.Migrations
{
    [Migration(22)]
    public class _0022_SeedAdmin : Migration
    {
        record Admin
        {
            Guid Id { get; set; }
            string Username { get; set; }
            string PasswordHash { get; set; }
            bool IsActive { get; set; }

        }
        static PasswordHasher<Admin> hasher = new();

        public override void Down()
        {

        }

        public override void Up()
        {
            Insert.IntoTable(tableName: "Admin").Row(new
            {
                Id = Guid.Parse("a25201ea-b655-44e3-a967-321a383fe37b"),
                Username = "Admin",
                PasswordHash = hasher.HashPassword(new Admin(), "123"),
                IsActive = true
            });
        }
    }
}
