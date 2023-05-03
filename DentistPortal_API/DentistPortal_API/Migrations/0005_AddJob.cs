using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(5)]
    public class _0005_AddJob : Migration
    {
        public override void Down()
        {
            Delete.Table("Job");
        }

        public override void Up()
        {
            Create.Table("Job")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("JobTitle").AsString().NotNullable()
                .WithColumn("Description").AsString().NotNullable()
                .WithColumn("Salary").AsString().NotNullable()
                .WithColumn("OwnerIdDoctor").AsGuid().Nullable().ForeignKey("Dentist", "Id").OnDelete(System.Data.Rule.Cascade)//Cant both be empty check at API
                .WithColumn("IsActive").AsBoolean().NotNullable()
                .WithColumn("ContactEmail").AsString().NotNullable()
                .WithColumn("ContactNumber").AsString().Nullable()
                .WithColumn("Location").AsString().NotNullable()
                .WithColumn("Duration").AsString().NotNullable();
        }
    }
}
