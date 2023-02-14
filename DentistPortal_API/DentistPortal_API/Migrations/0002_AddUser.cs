using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(2)]
    public class _0002_AddUser : Migration
    {
        public override void Down()
        {
            Delete.Table("User");
        }

        public override void Up()
        {
            Create.Table("User")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("Username").AsString().NotNullable().Unique()
                .WithColumn("FirstName").AsString().NotNullable()
                .WithColumn("LastName").AsString().NotNullable()
                .WithColumn("PasswordHash").AsString().NotNullable()
                .WithColumn("ProfilePicture").AsString().Nullable()
                .WithColumn("Role").AsString().NotNullable()
                .WithColumn("RefreshTokenId").AsGuid().Nullable().ForeignKey("RefreshToken", "Id")
                .WithColumn("IsActive").AsBoolean().NotNullable();
        }
    }
}
