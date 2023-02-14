using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(7)]
    public class _0007_AddMedicalCase : Migration
    {
        public override void Down()
        {
            Delete.Table("MedicalCase");
        }

        public override void Up()
        {
            Create.Table("MedicalCase")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("PatientName").AsString().NotNullable()
                .WithColumn("PatientPhone").AsString().NotNullable()
                .WithColumn("PatientAge").AsString().NotNullable()
                .WithColumn("Description").AsString().NotNullable()
                .WithColumn("DoctorId").AsGuid().NotNullable().ForeignKey("User", "Id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("IsActive").AsBoolean().NotNullable();
        }
    }
}
