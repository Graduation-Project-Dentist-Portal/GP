using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(6)]
    public class _0006_AddPost : Migration
    {
        public override void Down()
        {
            Delete.Table("Post");
        }

        public override void Up()
        {
            Create.Table("Post")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("Description").AsString().NotNullable()
                .WithColumn("OwnerIdDoctor").AsGuid().Nullable().ForeignKey("Dentist", "Id").OnDelete(System.Data.Rule.Cascade)//Cant both be empty check at API
                .WithColumn("OwnerIdPatient").AsGuid().Nullable().ForeignKey("Patient", "Id").OnDelete(System.Data.Rule.Cascade)//Cant both be empty check at API
                .WithColumn("PicturePaths").AsString().NotNullable()
                .WithColumn("IsActive").AsBoolean().NotNullable();
        }
    }
}
