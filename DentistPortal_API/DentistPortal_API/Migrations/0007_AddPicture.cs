using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(7)]
    public class _0007_AddPicture : Migration
    {
        public override void Down()
        {
            Delete.Table("Picture");
        }

        public override void Up()
        {
            Create.Table("Picture")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("PicturePath").AsString().NotNullable()
                .WithColumn("OwnerId").AsGuid().NotNullable().ForeignKey("User", "Id").ForeignKey("Post", "Id").ForeignKey("MedicalCase", "Id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("IsActive").AsBoolean().NotNullable();
        }
    }
}
