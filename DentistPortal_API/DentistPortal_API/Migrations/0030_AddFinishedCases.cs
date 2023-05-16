using FluentMigrator;

namespace DentistPortal_API.Migrations
{
    [Migration(30)]
    public class _0030_AddFinishedCases : Migration
    {
        public override void Down()
        {
            Delete.Table("FinishedCases");
        }

        //    public override void Up()
        //    {
        //        Create.Table("FinishedCases")
        //.WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
        //.WithColumn("DoctorWork").AsString().NotNullable()
        //.WithColumn("DoctorId").AsGuid().NotNullable()
        //.WithColumn("BeforePicture").AsString().NotNullable()
        //.WithColumn("AfterPicture").AsString().NotNullable()
        //.WithColumn("CaseId").AsGuid().NotNullable()
        //.WithColumn("IsMedicalCase").AsBoolean().NotNullable().WithDefaultValue(false)
        //.WithColumn("FinishedFK").AsGuid().Nullable()
        //.ForeignKey("PatientCase", "Id").OnDelete(System.Data.Rule.Cascade).OnUpdate(System.Data.Rule.Cascade).Indexed("IX_FinishedCases_CaseId_PatientCase").Nullable()
        //.ForeignKey("MedicalCase", "Id").OnDelete(System.Data.Rule.Cascade).OnUpdate(System.Data.Rule.Cascade).Indexed("IX_FinishedCases_CaseId_MedicalCase").Nullable();

        //        // Add a foreign key constraint for PatientCase
        //        Create.ForeignKey("FK_FinishedCases_FinishedFK_PatientCase_Id_New")
        //.FromTable("FinishedCases").ForeignColumn("FinishedFK")
        //.ToTable("PatientCase").PrimaryColumn("Id");

        //        // Add a foreign key constraint for MedicalCase
        //        Create.ForeignKey("FK_FinishedCases_FinishedFK_MedicalCase_Id_New")
        //            .FromTable("FinishedCases").ForeignColumn("FinishedFK")
        //            .ToTable("MedicalCase").PrimaryColumn("Id");

        //        // Add a constraint to ensure that only one of the foreign key columns is not null
        //        Execute.Sql(@"
        //        ALTER TABLE [dbo].[FinishedCases] ADD CONSTRAINT [CK_FinishedCases_CaseId] CHECK (
        //            ([CaseId] IS NOT NULL AND [FinishedFK] IS NULL) OR
        //            ([CaseId] IS NULL AND [FinishedFK] IS NOT NULL)
        //        )
        //    ");
        //    }

        public override void Up()
        {
            Create.Table("FinishedCases")
                .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
                .WithColumn("DoctorWork").AsString().NotNullable()
                .WithColumn("DoctorId").AsGuid().NotNullable()
                .WithColumn("BeforePicture").AsString().NotNullable()
                .WithColumn("AfterPicture").AsString().NotNullable()
                 .WithColumn("CaseId").AsGuid().NotNullable()
                .WithColumn("MedicalCaseId").AsGuid().Nullable().ForeignKey("MedicalCase", "Id")
                .WithColumn("PatientCaseId").AsGuid().Nullable().ForeignKey("PatientCase", "Id")

                .WithColumn("IsActive").AsBoolean().NotNullable();

        }
    }
}
