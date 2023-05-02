using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(23)]
    public class _0023_SeedClinicImage : Migration
    {
        public override void Down()
        {
        }

        public override void Up()
        {
            Insert.IntoTable("ClinicImage").Row(new
            {
                Id = "C59DB064-4705-4469-9353-112665A7A048",
                Url = "https://res.cloudinary.com/djrj0pmt0/image/upload/v1683047906/benyamin-bohlouli-LQ698TTvGpA-unsplash_ihvpv4.jpg",
                ClinicId = "D98F7075-795E-4F7F-A31F-155590479518",
                IsActive = true
            }).Row(new
            {
                Id = "800821E7-158C-4AAE-8B37-5D9DFDC2D86C",
                Url = "https://res.cloudinary.com/djrj0pmt0/image/upload/v1683047897/benyamin-bohlouli-e7MJLM5VGjY-unsplash_zm1q9f.jpg",
                ClinicId = "D98F7075-795E-4F7F-A31F-155590479518",
                IsActive = true
            }).Row(new
            {
                Id = "7751c86d-ade6-4616-a46c-6a2ee5fe9a8d",
                Url = "https://res.cloudinary.com/djrj0pmt0/image/upload/v1683047904/atikah-akhtar-XJptUS8nbhs-unsplash_cuuwrg.jpg",
                ClinicId = "21091AC0-7749-4FC0-BE14-E1895AA11BAD",
                IsActive = true
            });
        }
    }
}
