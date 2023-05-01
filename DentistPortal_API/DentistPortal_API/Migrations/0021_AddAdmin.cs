using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(21)]
    public class _0021_AddAdmin : Migration
    {
        public override void Down()
        {
            Delete.Table("Admin");
        }

        public override void Up()
        {
            Create.Table("Admin")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("Username").AsString().Unique().NotNullable()
                .WithColumn("PasswordHash").AsString().NotNullable()
                .WithColumn("IsActive").AsBoolean().NotNullable();
        }
    }
}
