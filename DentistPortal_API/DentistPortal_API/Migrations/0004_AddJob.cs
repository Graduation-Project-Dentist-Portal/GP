using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(4)]
    public class _0004_AddJob : Migration
    {
        public override void Down()
        {
            Delete.Table("job");
        }

        public override void Up()
        {
            Create.Table("job")
                .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("job_title").AsString().NotNullable()
                .WithColumn("description").AsString().NotNullable()
                .WithColumn("salary").AsFloat().NotNullable()
                .WithColumn("owner_id").AsGuid().NotNullable().ForeignKey("user", "id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("is_active").AsBoolean().NotNullable();
        }
    }
}
