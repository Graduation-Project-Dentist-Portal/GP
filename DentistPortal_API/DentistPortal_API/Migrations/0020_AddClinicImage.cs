using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(20)]
    public class _0020_AddClinicImage : Migration
    {
        public override void Down()
        {
            Delete.Table("ClinicImage");
        }

        public override void Up()
        {
            Create.Table("ClinicImage")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("Url").AsString().Unique().NotNullable()
                .WithColumn("ClinicId").AsGuid().ForeignKey("Clinic", "Id")
                .WithColumn("IsActive").AsBoolean().NotNullable();
        }
    }
}
