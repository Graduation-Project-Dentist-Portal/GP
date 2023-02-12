using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(3)]
    public class _0003_AddToolForSale : Migration
    {
        public override void Down()
        {
            Delete.Table("tool_for_sale");
        }

        public override void Up()
        {
            Create.Table("tool_for_sale")
                .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("tool_name").AsString().NotNullable()
                .WithColumn("description").AsString().NotNullable()
                .WithColumn("tool_price").AsFloat().NotNullable()
                .WithColumn("seller_id").AsGuid().NotNullable().ForeignKey("user", "id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("is_active").AsBoolean().NotNullable();
        }
    }
}
