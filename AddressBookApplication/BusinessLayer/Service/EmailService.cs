using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using BusinessLayer.Interface;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmail(string toEmail, string subject, string token)
        {
            var smtpClient = new SmtpClient(_config["EmailSettings:SmtpServer"])
            {
                Port = Convert.ToInt32(_config["EmailSettings:SmtpPort"]), 
                Credentials = new NetworkCredential(
                    _config["EmailSettings:SmtpUser"], 
                    _config["EmailSettings:SmtpPass"]  
                ),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["EmailSettings:FromEmail"]),
                Subject = subject,
                Body = $"<h3>Your Password Reset Token</h3><p>Use this token to reset your password:</p><h2>{token}</h2>",
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
