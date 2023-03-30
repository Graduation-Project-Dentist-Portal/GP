using FluentMigrator;

namespace DentistPortal_API.Migrations
{
   // [Migration(16)]
    public class _0016_SeedFeedbacks : Migration
    {
        public override void Down()
        {
        }

        public override void Up()
        {
            //Insert.IntoTable("Feedback").Row(new
            //{
            //    Id = "53fe5bf7-bbea-409c-a9c5-432f26f5cad4",
            //    Comment = "first comment",
            //    AiScore = "test",
            //    ClinicId = "D98F7075-795E-4F7F-A31F-155590479518",
            //    UserId = "63c108fc-aaf8-4f5b-a795-7beb3f29d76a",
            //    Likes = 0,
            //    IsActive = true
            //}).Row(new
            //{
            //    Id = "5291c418-2def-4186-8dfa-b5883bd9aa48",
            //    Comment = "2nd comment",
            //    AiScore = "test",
            //    ClinicId = "D98F7075-795E-4F7F-A31F-155590479518",
            //    UserId = "63c108fc-aaf8-4f5b-a795-7beb3f29d76a",
            //    Likes = 0,
            //    IsActive = true
            //});
        }
    }
}
