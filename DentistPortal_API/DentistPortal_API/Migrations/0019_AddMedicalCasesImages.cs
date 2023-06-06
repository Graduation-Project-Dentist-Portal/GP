using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(19)]
    public class _0019_AddMedicalCasesImages : Migration
    {
        public override void Down()
        {
            Delete.Table("MedicalCaseImage");
        }

        public override void Up()
        {
            Create.Table("MedicalCaseImage")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("Url").AsString().Unique().NotNullable()
                .WithColumn("MedicalCaseId").AsGuid().ForeignKey("MedicalCase", "Id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("IsActive").AsBoolean().NotNullable();
        }
    }
}
