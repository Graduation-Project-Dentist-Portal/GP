using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;

namespace DentistPortal_API.Controllers
{
    public class EmailController : Controller
    {
        //private readonly IConfiguration _configuration;

        //public EmailController(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}

        public async Task SendEmail(string body, string toEmail, IConfiguration _configuration)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration.GetSection("EMAIL").GetSection("email").Value));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = "DentalHub Registration";
            email.Body = new TextPart(TextFormat.Text) { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_configuration.GetSection("EMAIL").GetSection("email").Value, _configuration.GetSection("EMAIL").GetSection("password").Value);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
