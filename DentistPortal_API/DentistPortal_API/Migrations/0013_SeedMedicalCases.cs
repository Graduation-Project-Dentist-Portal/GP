using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(13)]
    public class _0013_SeedMedicalCases : Migration
    {
        public override void Down()
        {
        }

        public override void Up()
        {
            Insert.IntoTable("MedicalCase").Row(new
            {
                Id = Guid.Parse("4A00EC03-BEAE-4101-A4EF-11C009D25CFA"),
                Description = "Younes Yehia Case Description",
                PatientName = "Younes Yehia",
                PatientPhone = "01006465085",
                PatientAge = "21",
                DoctorId = Guid.Parse("0b89bb77-33ef-471d-8390-59b3969d86ae"),
                IsActive = true,
                Diagnosis = "Younes Yehia Case Diagnosis",
                CaseStatus = "Open",
                TimeCreated = "2023-03-02 11:54:53.670"
            }).Row(new
            {
                Id = Guid.Parse("7433ED0C-D0BB-4801-845A-8BE1DE170CB7"),
                Description = "Moataz Rafaat Case Description",
                PatientName = "Moataz Rafaat",
                PatientPhone = "01020769203",
                PatientAge = "22",
                DoctorId = Guid.Parse("e0406609-ed2e-466d-87b8-7b8fb1c34800"),
                IsActive = true,
                Diagnosis = "Moataz Rafaat Case Diagnosis",
                CaseStatus = "Open",
                TimeCreated = "2023-03-02 11:56:37.537"
            });
        }
    }
}
