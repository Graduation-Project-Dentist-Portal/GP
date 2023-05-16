using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(29)]
    public class _0029_AddPatientCasesImages : Migration
    {
        public override void Down()
        {
            Delete.Table("PatientCaseImage");
        }

        public override void Up()
        {
            Create.Table("PatientCaseImage")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("Url").AsString().Unique().NotNullable()
                .WithColumn("PatientCaseId").AsGuid().ForeignKey("PatientCase", "Id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("IsActive").AsBoolean().NotNullable();
        }
    }
}
