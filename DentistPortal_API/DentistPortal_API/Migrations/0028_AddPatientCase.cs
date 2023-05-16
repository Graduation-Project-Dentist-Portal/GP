 using FluentMigrator;
namespace DentistPortal_API.Migrations
{
    [Migration(28)]
    public class _0028_AddPatientCase : Migration
{
       
        public override void Down()
        {
            Delete.Table("PatientCase");
        }

        public override void Up()
        {
            Create.Table("PatientCase")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("PatientPhone").AsString().NotNullable()
                .WithColumn("PatientAge").AsInt32().NotNullable()
                .WithColumn("Description").AsString().NotNullable()
                .WithColumn("PatientId").AsGuid().NotNullable().ForeignKey("Patient", "Id").OnDelete(System.Data.Rule.Cascade)
                
                .WithColumn("TimeCreated").AsDateTime().NotNullable()
                .WithColumn("CaseStatus").AsString().NotNullable()
                .WithColumn("AssignedDoctorId").AsGuid().Nullable()
                .WithColumn("IsActive").AsBoolean().NotNullable();
        }
    }
}
