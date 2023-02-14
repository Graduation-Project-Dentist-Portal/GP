using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(9)]
    public class _0009_AddClinic : Migration
    {
        public override void Down()
        {
            Delete.Table("Clinic");
        }

        public override void Up()
        {
            Create.Table("Clinic")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("Address").AsString().NotNullable().Unique()
                .WithColumn("DoctorId").AsGuid().NotNullable().ForeignKey("User", "Id")
                .WithColumn("ClinicPhone").AsString().NotNullable();
        }
    }
}
