using DentistPortal_API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DentistPortal_API.Data
{
    public class WebsiteDbContext : DbContext
    {
        public WebsiteDbContext(DbContextOptions<WebsiteDbContext> options) : base(options) { }
        public DbSet<Dentist> Dentist { get; set; }
        public DbSet<Patient> Patient { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<MedicalCase> MedicalCase { get; set; }
        public DbSet<FinishedCases> FinishedCases { get; set; }
        public DbSet<Clinic> Clinic { get; set; }
        public DbSet<Feedback> Feedback { get; set; }
        public DbSet<Like> Like { get; set; }
        public DbSet<Tool> Tool { get; set; }
        public DbSet<ToolImage> ToolImage { get; set; }
        public DbSet<ClinicImage> ClinicImage { get; set; }
        public DbSet<MedicalCaseImage> MedicalCaseImage { get; set; }
    }
}
