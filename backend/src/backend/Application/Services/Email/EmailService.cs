using Application.DTOs.Email;
using Application.Interfaces.Email;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Application.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IEmailRepository _emailRepository;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IEmailRepository emailRepository, ILogger<EmailService> logger)
        {
            _emailRepository = emailRepository ?? throw new ArgumentNullException(nameof(emailRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> SendAsync(MailData mail)
        {
            if (mail == null)
            {
                _logger.LogWarning("EmailService.SendAsync called with null MailData.");
                return false;
            }

            try
            {
                // Bạn có thể thêm validation hoặc templating ở đây
                var result = await _emailRepository.SendAsync(mail);
                if (!result)
                {
                    _logger.LogWarning("EmailService: gửi email không thành công cho {To}", mail.EmailToId);
                }
                else
                {
                    _logger.LogInformation("EmailService: gửi email thành công cho {To}", mail.EmailToId);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EmailService: Exception when sending email to {To}", mail.EmailToId);
                return false;
            }
        }
    }
}
