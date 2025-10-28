using Application.DTOs.Email;
using Application.Interfaces.Email;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Email
{
    public class MailRepository : IEmailRepository
    {
        private readonly MailSettings _mailSettings;

        public MailRepository(IOptions<MailSettings> options)
        {
            _mailSettings = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<bool> SendAsync(MailData mailData)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_mailSettings.Name, _mailSettings.EmailId));
                message.To.Add(new MailboxAddress(mailData.EmailToName ?? mailData.EmailToId, mailData.EmailToId));
                message.Subject = mailData.EmailSubject ?? string.Empty;

                var bodyBuilder = new BodyBuilder();
                // Nếu muốn gửi html: use HtmlBody
                if (!string.IsNullOrWhiteSpace(mailData.EmailBody))
                {
                    bodyBuilder.TextBody = mailData.EmailBody;
                    // bodyBuilder.HtmlBody = mailData.EmailBody; // nếu bạn gửi HTML
                }

                // attachments nếu có (nếu MailData chứa danh sách attachments, xử lý ở đây)

                message.Body = bodyBuilder.ToMessageBody();

                using var smtp = new SmtpClient();
                // ConnectAsync có overload (host, port, bool useSsl)
                await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, _mailSettings.UseSSL);
                // Authenticate nếu cần
                if (!string.IsNullOrWhiteSpace(_mailSettings.EmailId) &&
                    !string.IsNullOrWhiteSpace(_mailSettings.Password))
                {
                    await smtp.AuthenticateAsync(_mailSettings.EmailId, _mailSettings.Password);
                }

                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                // Bạn nên log exception ở infra (hoặc rethrow lên để service log)
                // ví dụ: _logger.LogError(ex, "MailRepository.SendAsync failed"); // nếu inject logger
                return false;
            }
        }
    }
}
