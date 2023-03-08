using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(17)]
    public class _0017_AddLike : Migration
    {
        public override void Down()
        {
            Delete.Table("Like");
        }

        public override void Up()
        {
            Create.Table("Like")
                .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
                .WithColumn("PatientId").AsGuid().NotNullable().ForeignKey("Patient", "Id")
                .WithColumn("FeedbackId").AsGuid().NotNullable().ForeignKey("Feedback", "Id")
                .WithColumn("IsActive").AsBoolean().NotNullable();
        }
    }
}
