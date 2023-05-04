using CloudinaryDotNet;
using DentistPortal_API.Model;
using FluentMigrator;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentistPortal_API.Migrations
{
    [Migration(26)]
    public class _0026_SeedToolImage : Migration
    {
        record Tool
        {
            public Guid Id { get; set; }
            public string Url { get; set; }
            public Guid ToolId { get; set; }
            public bool IsActive { get; set; }
        }

        public override void Down()
        {

        }

        public override void Up()
        {
            Insert.IntoTable("ToolImage").Row(new
            {
                Id = Guid.Parse("4995E410-01DC-47DC-A15E-1D9B1908324A"),
                Url = "http://res.cloudinary.com/djrj0pmt0/image/upload/v1683046926/ozdbs4xl0zjmdf5epucx.webp",
                ToolId = "E8444149-0445-4AC6-800E-E10866669660",
                IsActive = true
            }).Row(new
            {
                Id = Guid.Parse("4CFF360A-3BD8-4FC6-8D7F-7C26DE721B18"),
                Url = "http://res.cloudinary.com/djrj0pmt0/image/upload/v1683045018/xsmgumsdnuppxpoqhjrp.jpg",
                ToolId = "2B73D002-9DB9-4AAD-876A-EAC3F61491E3",
                IsActive = true
            }).Row(new
            {
                Id = Guid.Parse("4E6B2045-63FF-4A19-AC6D-9C7C9874676D"),
                Url = "http://res.cloudinary.com/djrj0pmt0/image/upload/v1683049047/pwi1qkis4gmtqupzizbe.jpg",
                ToolId = "DC281731-9C27-46F3-9195-0CA11B8F7EBA",
                IsActive = true
            }).Row(new
            {
                Id = Guid.Parse("A482B8C8-4D5D-4C83-B9CB-DAF765A85F5B"),
                Url = "http://res.cloudinary.com/djrj0pmt0/image/upload/v1683045733/i18sr3vkycu0facejrd3.jpg",
                ToolId = "DFFED01B-BEA6-4B1C-BA8B-75D8AD01E63D",
                IsActive = true
            });
        }
    }
}
