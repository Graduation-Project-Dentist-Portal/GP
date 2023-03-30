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
                .WithColumn("PatientAge").AsInt32().NotNullable()
                .WithColumn("Description").AsString().NotNullable()
                .WithColumn("DoctorId").AsGuid().NotNullable().ForeignKey("Dentist", "Id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("Diagnosis").AsString().NotNullable()
                .WithColumn("TimeCreated").AsDateTime().NotNullable()
                .WithColumn("CaseStatus").AsString().NotNullable()
                .WithColumn("AssignedDoctorId").AsGuid().Nullable()
                .WithColumn("IsActive").AsBoolean().NotNullable();
        }
    }
}
