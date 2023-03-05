using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(14)]
    public class _0014_AddFinishedCases : Migration
    {
        public override void Down()
        {
            Delete.Table("FinishedCases");
        }

        public override void Up()
        {
            Create.Table("FinishedCases")
                .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
                .WithColumn("DoctorWork").AsString(int.MaxValue).NotNullable()
                .WithColumn("DoctorId").AsGuid().NotNullable()
                .WithColumn("BeforePicture").AsString(int.MaxValue).NotNullable()
                .WithColumn("AfterPicture").AsString(int.MaxValue).NotNullable()
                .WithColumn("CaseId").AsGuid().NotNullable().ForeignKey("MedicalCase", "Id");
        }
    }
}
