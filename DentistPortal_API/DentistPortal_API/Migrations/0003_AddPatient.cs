using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(3)]
    public class _0003_AddPatient : Migration
    {
        public override void Down()
        {
            Delete.Table("Patient");
        }

        public override void Up()
        {
            Create.Table("Patient")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("Username").AsString().NotNullable().Unique()
                .WithColumn("FirstName").AsString().NotNullable()
                .WithColumn("LastName").AsString().NotNullable()
                .WithColumn("PasswordHash").AsString().NotNullable()
                .WithColumn("ProfilePicture").AsString(int.MaxValue).Nullable()
                .WithColumn("RefreshTokenId").AsGuid().Nullable().ForeignKey("RefreshToken", "Id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("IsActive").AsBoolean().NotNullable();
        }
    }
}
