using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(5)]
    public class _0005_AddPost : Migration
    {
        public override void Down()
        {
            Delete.Table("post");
        }

        public override void Up()
        {
            Create.Table("post")
                .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("description").AsString().NotNullable()
                .WithColumn("user_id").AsGuid().NotNullable().ForeignKey("user", "id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("is_active").AsBoolean().NotNullable();
        }
    }
}
