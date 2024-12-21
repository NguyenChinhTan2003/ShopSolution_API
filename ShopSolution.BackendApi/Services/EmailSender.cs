using System.Net.Mail;
using System.Net;

namespace ShopSolution.BackendApi.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var smtpHost = emailSettings["SmtpHost"];
            var smtpPort = int.Parse(emailSettings["SmtpPort"]);
            var senderEmail = emailSettings["SenderEmail"];
            var senderPassword = emailSettings["SenderPassword"];

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderEmail, senderPassword)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (SmtpException ex)
            {
                // Log lỗi hoặc xử lý tùy ý
                throw new InvalidOperationException("Gửi email thất bại.", ex);
            }
        }
    }
}