using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(10)]
    public class _0010_AddFeedback : Migration
    {
        public override void Down()
        {
            Delete.Table("Feedback");
        }

        public override void Up()
        {
            Create.Table("Feedback")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("Comment").AsString().NotNullable()
                .WithColumn("ClinicId").AsGuid().NotNullable().ForeignKey("Clinic", "Id");
        }
    }
}
