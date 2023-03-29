using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(4)]
    public class _0004_AddTool : Migration
    {
        public override void Down()
        {
            Delete.Table("Tool");
        }

        public override void Up()
        {
            Create.Table("Tool")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("ToolName").AsString().NotNullable()
                .WithColumn("Description").AsString().NotNullable()
                .WithColumn("ToolStatus").AsString().NotNullable()
                .WithColumn("SellerLocation").AsString().NotNullable()
                .WithColumn("ContactNumber").AsString().NotNullable()
                .WithColumn("ToolPrice").AsDouble().NotNullable()
                .WithColumn("SellerIdDoctor").AsGuid().NotNullable().ForeignKey("Dentist", "Id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("PicturePaths").AsString(int.MaxValue).NotNullable()
                .WithColumn("IsActive").AsBoolean().NotNullable();
        }
    }
}
