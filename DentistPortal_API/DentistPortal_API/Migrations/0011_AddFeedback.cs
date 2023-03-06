using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(11)]
    public class _0011_AddFeedback : Migration
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
                .WithColumn("ClinicId").AsGuid().NotNullable().ForeignKey("Clinic", "Id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("UserId").AsGuid().NotNullable().ForeignKey("Patient", "Id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("AiScore").AsString().NotNullable()
                .WithColumn("IsActive").AsBoolean().NotNullable();
        }
    }
}
