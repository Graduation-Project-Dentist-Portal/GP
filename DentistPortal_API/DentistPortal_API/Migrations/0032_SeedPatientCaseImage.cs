using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(32)]
    public class _0032_SeedPatientCaseImage : Migration
    {
        public override void Down()
        {
        }

        public override void Up()
        {
            Insert.IntoTable("PatientCaseImage").Row(new
            {
                Id = "6B00E63F-6E0B-45AB-A2BD-4076D63A4887",
                Url = "http://res.cloudinary.com/djrj0pmt0/image/upload/v1684237956/y5wqyirhcqke5teun4i1.jpg",
                PatientCaseId = "9A5BB205-09AA-4ED0-9F9C-C00679A5130A",
                IsActive = true
            }).Row(new
            {
                Id = "EEFE117B-390A-4D3A-8A32-5F82592F4A69",
                Url = "http://res.cloudinary.com/djrj0pmt0/image/upload/v1684238214/qoxz5fnaetv1xdsepqdp.jpg",
                PatientCaseId = "FB40DDC6-BBF2-4F01-B1E6-D13A81328144",
                IsActive = true
            });
        }
    }
}
