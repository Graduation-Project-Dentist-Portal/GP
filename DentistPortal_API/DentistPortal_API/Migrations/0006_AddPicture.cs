using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(6)]
    public class _0006_AddPicture : Migration
    {
        public override void Down()
        {
            Delete.Table("picture");
        }

        public override void Up()
        {
            Create.Table("picture")
                .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("picture_path").AsString().NotNullable()
                .WithColumn("owner_id").AsGuid().NotNullable().ForeignKey("user", "id").ForeignKey("post", "id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("is_active").AsBoolean().NotNullable();
        }
    }
}
