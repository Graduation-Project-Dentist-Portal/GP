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
                .WithColumn("Address").AsString().NotNullable().Unique()
                .WithColumn("DoctorId").AsGuid().NotNullable().ForeignKey("Dentist", "Id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("ClinicPhone").AsString().NotNullable()
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("OpenTime").AsInt32().NotNullable()
                .WithColumn("CloseTime").AsInt32().NotNullable()
                .WithColumn("ClinicDescription").AsString().NotNullable()
                .WithColumn("PicturePaths").AsString(int.MaxValue).NotNullable()
                .WithColumn("IsActive").AsBoolean().NotNullable();
        }
    }
}
