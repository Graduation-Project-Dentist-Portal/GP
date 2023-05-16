using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(30)]
    public class _0030_AddFinishedCases : Migration
    {
        public override void Down()
        {
            Delete.Table("FinishedCases");
        }

        public override void Up()
        {
            Create.Table("FinishedCases")
                .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
                .WithColumn("DoctorWork").AsString().NotNullable()
                .WithColumn("DoctorId").AsGuid().NotNullable()
                .WithColumn("BeforePicture").AsString().NotNullable()
                .WithColumn("AfterPicture").AsString().NotNullable()
                .WithColumn("CaseId").AsGuid().NotNullable()
                .WithColumn("MedicalCaseId").AsGuid().Nullable().ForeignKey("MedicalCase", "Id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("PatientCaseId").AsGuid().Nullable().ForeignKey("PatientCase", "Id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("IsActive").AsBoolean().NotNullable();
        }
    }
}
