using DentistPortal_API.Model;
using FluentMigrator;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentistPortal_API.Migrations
{
    [Migration(25)]
    public class _0025_SeedTools : Migration
    {
        record Tool
        {
            Guid Id { get; set; }
            string ToolName { get; set; }
            string Description { get; set; }
            double ToolPrice { get; set; }
            string SellerLocation { get; set; }
            string ContactNumber { get; set; }
            Guid SellerIdDoctor { get; set; }
            bool IsActive { get; set; }
            string ToolStatus { get; set; }
        }

        public override void Down()
        {

        }

        public override void Up()
        {
            Insert.IntoTable("Tool").Row(new
            {
                Id = Guid.Parse("DC281731-9C27-46F3-9195-0CA11B8F7EBA"),
                ToolName = "ultra sonic tooth cleaner",
                Description = "Break apart and remove the plaque and calculus that has built up on your teeth. Moreover, a small stream of water comes out of the tip and helps blast away additional buildup while washing away the debris.",
                ToolStatus = "Available",
                SellerLocation = "Zayed",
                ContactNumber = "045678",
                ToolPrice = "400",
                SellerIdDoctor = Guid.Parse("0b89bb77-33ef-471d-8390-59b3969d86ae"),
                IsActive = true

            }).Row(new
            {
                Id = Guid.Parse("DFFED01B-BEA6-4B1C-BA8B-75D8AD01E63D"),
                ToolName = "Tooth Extraction forceps",
                Description = "Prestige Tooth Exratcing forceps No. 24 (Polished finished )\r\nHigh quality Medical Standard Stainless Steel",
                ToolStatus = "Available",
                SellerLocation = "Maadi",
                ContactNumber = "0139202053",
                ToolPrice = "300",
                SellerIdDoctor = Guid.Parse("0b89bb77-33ef-471d-8390-59b3969d86ae"),
                IsActive = true
            }).Row(new
            {
                Id = Guid.Parse("E8444149-0445-4AC6-800E-E10866669660"),
                ToolName = "Scaler",
                Description = "A scaler helps tackle oral issues like plaque buildup, periodontal disease, and other forms of buildups that cannot be scraped with a probe. Most of these buildups are trapped in tiny pockets between teeth.",
                ToolStatus = "Available",
                SellerLocation = "Zayed",
                ContactNumber = "0112222",
                ToolPrice = "400",
                SellerIdDoctor = Guid.Parse("0b89bb77-33ef-471d-8390-59b3969d86ae"),
                IsActive = true

            }).Row(new
            {
                Id = Guid.Parse("2B73D002-9DB9-4AAD-876A-EAC3F61491E3"),
                ToolName = "Dental Calculus Remover",
                Description = "Quickly and effectively removes calculus/tartar/dental plaque on teeth surface and between teeth via highly Precise Design. Comes with Double ended Sharped and spacialy Desiegned points.",
                ToolStatus = "Available",
                SellerLocation = "Maadi",
                ContactNumber = "01092500431",
                ToolPrice = "350",
                SellerIdDoctor = Guid.Parse("e0406609-ed2e-466d-87b8-7b8fb1c34800"),
                IsActive = true
            });
        }
    }
}
