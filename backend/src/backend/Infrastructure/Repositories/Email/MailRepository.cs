using Application.DTOs.Email;
using Application.Interfaces.Email;
using Infrastructure.Setting;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Email
{
    public class MailRepository : IEmailRepository
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<MailRepository> _logger;

        public MailRepository(IOptions<MailSettings> options, ILogger<MailRepository> logger)
        {
            _mailSettings = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
        }

        public async Task<bool> SendAsync(MailData mailData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_mailSettings.EmailId))
                {
                    throw new ValidationException("MailSettings:EmailId chưa được cấu hình.");
                }

                if (string.IsNullOrWhiteSpace(mailData.EmailToId))
                {
                    throw new ValidationException("Email người nhận (EmailToId) không được để trống.");
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_mailSettings.Name, _mailSettings.EmailId));
                message.To.Add(new MailboxAddress(mailData.EmailToName ?? mailData.EmailToId, mailData.EmailToId));
                message.Subject = mailData.EmailSubject ?? string.Empty;

                var bodyBuilder = new BodyBuilder();
                if (!string.IsNullOrWhiteSpace(mailData.EmailBody))
                {
                    
                    bodyBuilder.HtmlBody = mailData.EmailBody; 
                }

                message.Body = bodyBuilder.ToMessageBody();

                using var smtp = new SmtpClient();

                // 1. Bỏ qua lỗi chứng chỉ
                smtp.ServerCertificateValidationCallback =
                    (sender, certificate, chain, sslPolicyErrors) => true;

                // 2. Ép dùng TLS 1.2
                smtp.SslProtocols = SslProtocols.Tls12;

                // 3. Dùng StartTls tường minh
                await smtp.ConnectAsync(
                    _mailSettings.Host,
                    _mailSettings.Port,
                    SecureSocketOptions.StartTls);

                if (!string.IsNullOrWhiteSpace(_mailSettings.UserName) &&
                    !string.IsNullOrWhiteSpace(_mailSettings.Password))
                {
                    // Dùng UserName
                    await smtp.AuthenticateAsync(_mailSettings.UserName, _mailSettings.Password);
                }

                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MailRepository.SendAsync thất bại. Gửi đến: {EmailToId}", mailData?.EmailToId);
                return false;
            }
        }
    }
}