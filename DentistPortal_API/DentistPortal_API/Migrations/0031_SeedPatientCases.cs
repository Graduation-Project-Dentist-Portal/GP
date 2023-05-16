using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(31)]
    public class _0031_SeedPatientCases : Migration
    {
        public override void Down()
        {
        }

        public override void Up()
        {
            Insert.IntoTable("PatientCase").Row(new
            {
                Id = Guid.Parse("9A5BB205-09AA-4ED0-9F9C-C00679A5130A"),
                PatientPhone = "01006465085",
                PatientAge = "21",
                Description = "Case one Patient Description",
                PatientId = Guid.Parse("63C108FC-AAF8-4F5B-A795-7BEB3F29D76A"),
                TimeCreated = "2023-05-16 14:52:17.937",
                CaseStatus = "Open",
                IsActive = true
            }).Row(new
            {
                Id = Guid.Parse("FB40DDC6-BBF2-4F01-B1E6-D13A81328144"),
                PatientPhone = "01006465085",
                PatientAge = "21",
                Description = "Case Two Patient Description",
                PatientId = Guid.Parse("63C108FC-AAF8-4F5B-A795-7BEB3F29D76A"),
                TimeCreated = "2023-05-16 14:55:55.633",
                CaseStatus = "Open",
                IsActive = true
            });
        }
    }
}
