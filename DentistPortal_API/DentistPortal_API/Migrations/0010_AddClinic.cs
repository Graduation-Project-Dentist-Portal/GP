using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(10)]
    public class _0010_AddClinic : Migration
    {
        public override void Down()
        {
            Delete.Table("Clinic");
        }

        public override void Up()
        {
            Create.Table("Clinic")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("Address").AsString().NotNullable()
                .WithColumn("DoctorId").AsGuid().NotNullable().ForeignKey("Dentist", "Id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("ClinicPhone").AsString().NotNullable()
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("OpenTime").AsDateTime().NotNullable()
                .WithColumn("CloseTime").AsDateTime().NotNullable()
                .WithColumn("ClinicDescription").AsString().NotNullable()
                .WithColumn("Score").AsDouble().NotNullable()
                .WithColumn("IsActive").AsBoolean().NotNullable();
        }
    }
}
