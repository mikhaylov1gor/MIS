using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace MIS.Infrastucture.Email
{
    public interface IEmailSender
    {
        void SendEmail(string toEmail, string subject, string body);
    }
    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailSender(IOptions<SmtpSettings> options)
        {
            _smtpSettings = options.Value;
        }

        public async void SendEmail(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient("sandbox.smtp.mailtrap.io")
            {
                Port = 2525,
                Credentials = new NetworkCredential("8f0f6ff37a53a0", "d1884e0b0219c3"),
                EnableSsl = true
            };
            var email = new MailMessage
            {
                From = new MailAddress("no_reply@example.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            email.To.Add(toEmail);

            await smtpClient.SendMailAsync(email);
        }   
    }

    public class SmtpSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FromEmail { get; set; }
    }
}
