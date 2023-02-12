using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(2)]
    public class _0002_AddUser : Migration
    {
        public override void Down()
        {
            Delete.Table("user");
        }

        public override void Up()
        {
            Create.Table("user")
                .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("user_name").AsString().NotNullable().Unique()
                .WithColumn("first_name").AsString().NotNullable()
                .WithColumn("last_name").AsString().NotNullable()
                .WithColumn("password_hash").AsString().NotNullable()
                .WithColumn("profile_picture").AsString().NotNullable()
                .WithColumn("role").AsString().NotNullable()
                .WithColumn("refresh_token_id").AsGuid().Nullable().ForeignKey("refresh_token", "id")
                .WithColumn("is_active").AsBoolean().NotNullable();
        }
    }
}
