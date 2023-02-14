using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(3)]
    public class _0003_AddToolForSale : Migration
    {
        public override void Down()
        {
            Delete.Table("ToolForSale");
        }

        public override void Up()
        {
            Create.Table("ToolForSale")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("ToolName").AsString().NotNullable()
                .WithColumn("Description").AsString().NotNullable()
                .WithColumn("ToolPrice").AsFloat().NotNullable()
                .WithColumn("SellerId").AsGuid().NotNullable().ForeignKey("User", "Id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("IsActive").AsBoolean().NotNullable();
        }
    }
}
