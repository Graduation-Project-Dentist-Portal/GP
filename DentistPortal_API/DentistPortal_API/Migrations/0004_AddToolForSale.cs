using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(4)]
    public class _0004_AddToolForSale : Migration
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
                .WithColumn("SellerId").AsGuid().NotNullable().ForeignKey("Dentist", "Id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("PicturePaths").AsString(int.MaxValue).NotNullable()
                .WithColumn("IsActive").AsBoolean().NotNullable();
        }
    }
}
