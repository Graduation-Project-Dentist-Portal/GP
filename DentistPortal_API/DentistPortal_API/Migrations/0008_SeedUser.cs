using FluentMigrator;
using Microsoft.AspNetCore.Identity;

namespace DentistPortal_API.Migrations
{
    [Migration(8)]
    public class _0008_SeedUser : Migration
    {
        record user
        {
            public Guid id { get; set; }
            public string user_name { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string password_hash { get; set; }
            public Guid refresh_token_id { get; set; }
            public string profile_picture { get; set; }
            public string role { get; set; }
            public bool is_active { get; set; }
        }
        static PasswordHasher<user> hasher = new();
        public override void Down()
        {
        }

        public override void Up()
        {
            Insert.IntoTable(tableName: "user").Row(new
            {
                id = Guid.Parse("0b89bb77-33ef-471d-8390-59b3969d86ae"),
                user_name = "YounesAbady",
                first_name = "Younes",
                last_name = "Abady",
                password_hash = hasher.HashPassword(new user(), "123"),
                role = "Dentist",
                is_active = true,
                profile_picture="asd"
            }).Row(new
            {
                id = Guid.Parse("63c108fc-aaf8-4f5b-a795-7beb3f29d76a"),
                user_name = "HagarMohamed",
                first_name = "Hagar",
                last_name = "Mohamed",
                password_hash = hasher.HashPassword(new user(), "123"),
                role = "Student",
                is_active = true,
                profile_picture = "asd"
            }).Row(new
            {
                id = Guid.Parse("1cc48874-ac1c-468d-b25b-d6af0425b86d"),
                user_name = "MohamedKhaled",
                first_name = "Mohamed",
                last_name = "Khaled",
                password_hash = hasher.HashPassword(new user(), "123"),
                role = "Other",
                is_active = true,
                profile_picture = "asd"
            });
        }
    }
}
