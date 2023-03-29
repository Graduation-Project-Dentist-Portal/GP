using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace DentistPortal_API.Services
{
    public class Globals
    {
        private static IConfiguration _config = new ConfigurationBuilder()
       .AddJsonFile("appsettings.json")
       .AddEnvironmentVariables()
       .Build();

        //public static async Task<string> UploadImage(IFormFile file)
        //{
        //    Account account = new Account(_config.GetSection("CLOUDINARY_URL").GetSection("cloudinary_name").Value,
        //                                  _config.GetSection("CLOUDINARY_URL").GetSection("cloudinary_name").Value,
        //                                  _config.GetSection("CLOUDINARY_URL").GetSection("cloudinary_name").Value);
        //    Cloudinary cloudinary = new Cloudinary(account);
        //    cloudinary.Api.Secure = true;

        //    var uploadParams = new ImageUploadParams()
        //    {
        //        File = new FileDescription(file.FileName)
        //    };
        //    return await cloudinary.UploadAsync(uploadParams);
        //}
    }
}
