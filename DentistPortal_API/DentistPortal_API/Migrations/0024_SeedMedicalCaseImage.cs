using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(24)]
    public class _0024_SeedMedicalCaseImage : Migration
    {
        public override void Down()
        {
        }

        public override void Up()
        {
            Insert.IntoTable("MedicalCaseImage").Row(new
            {
                Id = "b83e7ac0-bf8c-4df0-9b29-0db77b0d4847",
                Url = "https://res.cloudinary.com/djrj0pmt0/image/upload/v1683062513/1_z9zamp.jpg",
                MedicalCaseId = "4A00EC03-BEAE-4101-A4EF-11C009D25CFA",
                IsActive = true
            }).Row(new
            {
                Id = "dcb508bd-afc7-41c0-bfe1-536aa05e1278",
                Url = "https://res.cloudinary.com/djrj0pmt0/image/upload/v1683062513/2_ok3xhx.jpg",
                MedicalCaseId = "4A00EC03-BEAE-4101-A4EF-11C009D25CFA",
                IsActive = true
            }).Row(new
            {
                Id = "3361eb47-6635-48cc-b2a8-6f020ee13ec1",
                Url = "https://res.cloudinary.com/djrj0pmt0/image/upload/v1683062513/3_tj9fqx.jpg",
                MedicalCaseId = "4A00EC03-BEAE-4101-A4EF-11C009D25CFA",
                IsActive = true
            }).Row(new
            {
                Id = "4d562798-1d10-4cea-b9a4-c75e83f2c546",
                Url = "https://res.cloudinary.com/djrj0pmt0/image/upload/v1683062513/4_mb0ro4.jpg",
                MedicalCaseId = "4A00EC03-BEAE-4101-A4EF-11C009D25CFA",
                IsActive = true
            }).Row(new
            {
                Id = "16c74a64-e312-47f7-ab02-0154d763c892",
                Url = "https://res.cloudinary.com/djrj0pmt0/image/upload/v1683062513/5_m6p9vg.jpg",
                MedicalCaseId = "4A00EC03-BEAE-4101-A4EF-11C009D25CFA",
                IsActive = true
            }).Row(new
            {
                Id = "943ad6d0-71ad-45f8-8e39-3cd866b4e1fb",
                Url = "https://res.cloudinary.com/djrj0pmt0/image/upload/v1683062513/6_vjmpzn.jpg",
                MedicalCaseId = "7433ED0C-D0BB-4801-845A-8BE1DE170CB7",
                IsActive = true
            }).Row(new
            {
                Id = "6f4cc40a-2a27-4ebd-9003-87acc43ee6d8",
                Url = "https://res.cloudinary.com/djrj0pmt0/image/upload/v1683062513/7_zdvi5p.jpg",
                MedicalCaseId = "7433ED0C-D0BB-4801-845A-8BE1DE170CB7",
                IsActive = true
            }).Row(new
            {
                Id = "9b111923-103d-42b7-9bd2-bbb8afeed52e",
                Url = "https://res.cloudinary.com/djrj0pmt0/image/upload/v1683062512/8_h4c6yc.jpg",
                MedicalCaseId = "7433ED0C-D0BB-4801-845A-8BE1DE170CB7",
                IsActive = true
            }).Row(new
            {
                Id = "571748c8-67a6-419f-a9d2-759ff971571e",
                Url = "https://res.cloudinary.com/djrj0pmt0/image/upload/v1683062513/9_t0m7uz.jpg",
                MedicalCaseId = "7433ED0C-D0BB-4801-845A-8BE1DE170CB7",
                IsActive = true
            }).Row(new
            {
                Id = "caf14312-ff18-41c8-b9ea-2cb650a7beb8",
                Url = "https://res.cloudinary.com/djrj0pmt0/image/upload/v1683062513/10_wlfpfc.jpg",
                MedicalCaseId = "7433ED0C-D0BB-4801-845A-8BE1DE170CB7",
                IsActive = true
            });
        }
    }
}
