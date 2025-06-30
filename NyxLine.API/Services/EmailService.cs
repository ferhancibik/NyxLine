using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace NyxLine.API.Services
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderPassword { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort);
                client.Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword);
                client.EnableSsl = true;

                var message = new MailMessage();
                message.From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
                message.To.Add(toEmail);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                // Eğer e-posta gönderilemezse konsola yazdır (geliştirme amaçlı)
                Console.WriteLine($"E-posta gönderimi başarısız: {ex.Message}");
                Console.WriteLine($"Şifre sıfırlama bağlantısı: {body}");
                throw;
            }
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken)
        {
            var resetLink = $"http://localhost:8080/reset-password?token={resetToken}";
            
            var subject = "NyxLine - Şifre Sıfırlama";
            var body = $@"
                <html>
                <body>
                    <h2>Şifre Sıfırlama Talebi</h2>
                    <p>Merhaba,</p>
                    <p>NyxLine hesabınız için şifre sıfırlama talebinde bulundunuz.</p>
                    <p>Şifrenizi sıfırlamak için aşağıdaki bağlantıya tıklayın:</p>
                    <p><a href='{resetLink}' style='background-color: #4CAF50; color: white; padding: 14px 20px; text-decoration: none; border-radius: 4px;'>Şifremi Sıfırla</a></p>
                    <p>Bu bağlantı 1 saat geçerlidir.</p>
                    <p>Eğer bu talebi siz yapmadıysanız, bu e-postayı dikkate almayın.</p>
                    <br>
                    <p>Saygılarımızla,<br>NyxLine Ekibi</p>
                    <hr>
                    <p><small>Token: {resetToken}</small></p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }
    }
} 