using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(4)]
    public class _0004_AddJob : Migration
    {
        public override void Down()
        {
            Delete.Table("Job");
        }

        public override void Up()
        {
            Create.Table("Job")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("JobTitle").AsString().NotNullable()
                .WithColumn("Description").AsString().NotNullable()
                .WithColumn("Salary").AsFloat().NotNullable()
                .WithColumn("OwnerId").AsGuid().NotNullable().ForeignKey("User", "Id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("IsActive").AsBoolean().NotNullable();
        }
    }
}
