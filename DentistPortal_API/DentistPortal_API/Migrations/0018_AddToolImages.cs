using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(18)]
    public class _0018_AddToolImages : Migration
    {
        public override void Down()
        {
            Delete.Table("ToolImage");
        }

        public override void Up()
        {
            Create.Table("ToolImage")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("Url").AsString().Unique().NotNullable()
                .WithColumn("ToolId").AsGuid().ForeignKey("Tool", "Id")
                .WithColumn("IsActive").AsBoolean().NotNullable();
        }
    }
}
