using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    //[Migration(13)]
    public class _0013_SeedPicture : Migration
    {
        record picture
        {
            public Guid Id { get; set; }
            public string PicturePath { get; set; } = String.Empty;
            public Guid OwnerId { get; set; }
            public bool IsActive { get; set; }
        }

        public override void Down()
        {
        }

        public override void Up()
        {
            Insert.IntoTable("Picture").Row(new
            {
                Id = "b8ffc245-ab90-4f16-a353-70ad8dce173f",
                PicturePath = "test",
                OwnerId = "0b89bb77-33ef-471d-8390-59b3969d86ae",
                IsActive = true
            });
        }
    }
}
