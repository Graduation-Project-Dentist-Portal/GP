using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(5)]
    public class _0005_AddPost : Migration
    {
        public override void Down()
        {
            Delete.Table("Post");
        }

        public override void Up()
        {
            Create.Table("Post")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("Description").AsString().NotNullable()
                .WithColumn("UserId").AsGuid().NotNullable().ForeignKey("User", "Id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("IsActive").AsBoolean().NotNullable();
        }
    }
}
