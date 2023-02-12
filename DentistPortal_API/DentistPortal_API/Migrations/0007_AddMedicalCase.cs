using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(7)]
    public class _0007_AddMedicalCase : Migration
    {
        public override void Down()
        {
            Delete.Table("medical_case");
        }

        public override void Up()
        {
            Create.Table("medical_case")
                .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("patient_name").AsString().NotNullable()
                .WithColumn("patient_phone").AsString().NotNullable()
                .WithColumn("patient_age").AsString().NotNullable()
                .WithColumn("description").AsString().NotNullable()
                .WithColumn("doctor_id").AsGuid().NotNullable().ForeignKey("user", "id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("is_active").AsBoolean().NotNullable();
        }
    }
}
