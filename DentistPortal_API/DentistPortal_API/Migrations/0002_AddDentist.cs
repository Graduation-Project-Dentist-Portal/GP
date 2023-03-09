using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(2)]
    public class _0002_AddDentist : Migration
    {
        public override void Down()
        {
            Delete.Table("Dentist");
        }

        public override void Up()
        {
            Create.Table("Dentist")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("Username").AsString().NotNullable().Unique()
                .WithColumn("FirstName").AsString().NotNullable()
                .WithColumn("LastName").AsString().NotNullable()
                .WithColumn("PasswordHash").AsString().NotNullable()
                .WithColumn("ProfilePicture").AsString(int.MaxValue).Nullable()
                .WithColumn("RefreshTokenId").AsGuid().Nullable().ForeignKey("RefreshToken", "Id")
                .WithColumn("IsActive").AsBoolean().NotNullable()
                .WithColumn("Graduated").AsBoolean().NotNullable()
                .WithColumn("University").AsString().NotNullable()
                .WithColumn("Level").AsInt32().NotNullable()
                .WithColumn("IdentityCardPicture").AsString(int.MaxValue).NotNullable()
                .WithColumn("UniversityCardPicture").AsString(int.MaxValue).NotNullable();
        }
    }
}
