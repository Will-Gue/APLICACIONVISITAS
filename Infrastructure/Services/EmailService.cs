using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Visitapp.Infrastructure.Services
{
    public class EmailService : Visitapp.Application.Interfaces.IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {

            var smtpHost = _configuration["Smtp:Host"];
            var smtpPort = int.Parse(_configuration["Smtp:Port"] ?? "587");
            var smtpUser = _configuration["Smtp:User"];
            var smtpPass = _configuration["Smtp:Pass"];
            var fromEmail = _configuration["Smtp:From"];
            if (string.IsNullOrWhiteSpace(fromEmail))
            {
                fromEmail = smtpUser ?? "noreply@localhost";
            }

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var mail = new MailMessage(fromEmail, to, subject, body)
            {
                IsBodyHtml = true
            };

            try
            {
                await client.SendMailAsync(mail);
                _logger.LogInformation("Email sent to {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {To}", to);
                throw;
            }
        }
    }
}
