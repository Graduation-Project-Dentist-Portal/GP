using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(27)]
    public class _0027_SeedJobs : Migration
    {
        public override void Down()
        {
        }

        public override void Up()
        {
            Insert.IntoTable(tableName: "Job").Row(new
            {
                Id = Guid.Parse("A55BEDC3-4BCB-4019-82D0-023206B3AEA6"),
                JobTitle = "Dentist",
                Description = "Dentist Job Description at dental care Egypt.",
                Salary = "15K",
                ContactEmail = "dentalcareegypt@gmail.com",
                ContactNumber = "01111111111",
                Location = "Nasr City",
                Duration = "2:00-8:00",
                OwnerIdDoctor = Guid.Parse("E0406609-ED2E-466D-87B8-7B8FB1C34800"),
                IsActive = true
            }).Row(new
            {
                Id = Guid.Parse("D9C945C9-718D-442F-AD80-3AAA140390FE"),
                JobTitle = "Dentist Assistant",
                Description = "Dentist under training for 3 months at smile clinics",
                Salary = "Negotiatable",
                ContactEmail = "smileclinics@gmail.com",
                ContactNumber = "01000000000",
                Location = "Maadi",
                Duration = "8 hours",
                OwnerIdDoctor = Guid.Parse("0B89BB77-33EF-471D-8390-59B3969D86AE"),
                IsActive = true
            }).Row(new
            {
                Id = Guid.Parse("552E0C01-3F46-438E-AC35-8E81239F97DD"),
                JobTitle = "Dental assistant",
                Description = "Dental assistant is to sanitize tools and help dentists during diagnosis.",
                Salary = "5000",
                ContactEmail = "mohamedxyz@gmail.com",
                ContactNumber = "01010101010",
                Location = "October",
                Duration = "10 fixed hours",
                OwnerIdDoctor = Guid.Parse("0B89BB77-33EF-471D-8390-59B3969D86AE"),
                IsActive = true
            }).Row(new
            {
                Id = Guid.Parse("AD3935DC-FBB3-4AB4-9420-D18A47A56F11"),
                JobTitle = "Pediatric Dental specialist",
                Description = "Diagnosis for children at happy tooth clinic",
                Salary = "9000",
                ContactEmail = "happytoothclinic@hotmail.com",
                ContactNumber = "0123456789",
                Location = "Fifth settlement",
                Duration = "2:00-10:00",
                OwnerIdDoctor = Guid.Parse("0B89BB77-33EF-471D-8390-59B3969D86AE"),
                IsActive = true
            }).Row(new
            {
                Id = Guid.Parse("413D2AB5-32EA-42CF-96F4-DA4C711FF8DC"),
                JobTitle = "Orthodontics specialist",
                Description = "Orthodontics specialist job description",
                Salary = "12K",
                ContactEmail = "white_teethclinics@gmail.com",
                ContactNumber = "0987654332",
                Location = "Zayed",
                Duration = "4:00-11:00",
                OwnerIdDoctor = Guid.Parse("E0406609-ED2E-466D-87B8-7B8FB1C34800"),
                IsActive = true
            });
        }
    }
}
