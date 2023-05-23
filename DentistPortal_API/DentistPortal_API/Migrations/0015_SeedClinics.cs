using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(15)]
    public class _0015_SeedClinics : Migration
    {
        public override void Down()
        {
        }

        public override void Up()
        {
            Insert.IntoTable("Clinic").Row(new
            {
                Id = "D98F7075-795E-4F7F-A31F-155590479518",
                Address = "Sayeda Zeinab - Cairo",
                DoctorId = "0B89BB77-33EF-471D-8390-59B3969D86AE",
                ClinicPhone = "01006465085",
                Name = "Oasis Clinics",
                OpenTime = DateTime.Parse("2023-05-02 09:00:00.000"),
                CloseTime = DateTime.Parse("2023-05-02 22:00:00.000"),
                ClinicDescription = "Oasis Clinics is a medical group practice specializing in outpatient and inpatient daycare health services." +
                " It is a one-of-a-kind polyclinic wholly devoted to outpatient services.",
                Score = 0,
                IsActive = true
            }).Row(new
            {
                Id = "21091AC0-7749-4FC0-BE14-E1895AA11BAD",
                Address = "Sidi Bishr - Alexandria",
                DoctorId = "0B89BB77-33EF-471D-8390-59B3969D86AE",
                ClinicPhone = "01000066654",
                Name = "Alexandria Capital Clinics",
                OpenTime = DateTime.Parse("2023-05-02 14:00:00.000"),
                CloseTime = DateTime.Parse("2023-05-02 23:00:00.000"),
                ClinicDescription = "Health care is a constant form of collaboration and innovation." +
                " There are always new fields to explore and new services to offer.",
                Score = 0,
                IsActive = true
            });
        }
    }
}
