using Application.Common;
using Application.DTOs.Contact;
using Application.DTOs.Email;
using Application.Exceptions;
using Application.Interfaces; // IUnitOfWork
using Application.Interfaces.Email; // IEmailRepository
using Application.Interfaces.Staff.Repository;
using Application.Interfaces.Staff.Service; // Nơi chứa IReplyContactService
using Domain.Entities;
using Microsoft.Extensions.Logging; // Dùng để ghi log lỗi
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.Staff
{
    public class ReplyContactService : IReplyContactService
    {
        private readonly IReplyContactRepository _contactRepository;
        private readonly IEmailRepository _emailRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReplyContactService> _logger;

        public ReplyContactService(
            IReplyContactRepository contactRepository,
            IEmailRepository emailRepository,
            IUnitOfWork unitOfWork,
            ILogger<ReplyContactService> logger)
        {
            _contactRepository = contactRepository;
            _emailRepository = emailRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> ReplyToContactAsync(long contactId, ReplyContactDTO dto, long staffId, CancellationToken cancellationToken = default)
        {
            // === BƯỚC 1: LẤY VÀ KIỂM TRA CONTACT ===

            if (string.IsNullOrWhiteSpace(dto.ReplyContent))
            {
                throw new Exception("Nội dung trả lời không được để trống.");
            }

            var contact = await _contactRepository.GetByIdAsync(contactId, cancellationToken);
            if (contact == null)
            {
                throw new NotFoundException($"Không tìm thấy liên hệ với ID: {contactId}");
            }

            bool alreadyReplied = await _contactRepository.IsContactRepliedAsync(contactId, cancellationToken);
            if (alreadyReplied)
            {
                throw new Exception("Liên hệ này đã được trả lời trước đó.");
            }

            if (string.IsNullOrEmpty(contact.Email))
            {
                throw new Exception("Không thể trả lời vì liên hệ này thiếu email của khách.");
            }

            // === BƯỚC 2: CẬP NHẬT DATABASE ===

            contact.ReplyContent = dto.ReplyContent;
            contact.ReplyById = staffId;
            contact.ReplyAt = DateTimeOffset.UtcNow;
            contact.Status = "Replied";
            contact.ClosedAt = DateTimeOffset.UtcNow;
            contact.IsReplySent = false;

            _contactRepository.Update(contact);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // === BƯỚC 3: TẠO TEMPLATE EMAIL HTML VÀ GỬI ===

            // Tạo nội dung HTML "đẹp"
            string emailHtmlBody = $@"
<div style='font-family: Arial, sans-serif; font-size: 16px; line-height: 1.6; color: #333;'>
    <p>Chào bạn,</p>
    <p>Chúng tôi đã nhận được liên hệ của bạn về chủ đề: <strong>""{contact.Title}""</strong>.</p>
    <p>Nhân viên hỗ trợ của chúng tôi đã trả lời như sau:</p>
    
    <div style='background-color: #f9f9f9; border: 1px solid #ddd; border-radius: 5px; padding: 20px; margin: 20px 0;'>
        {dto.ReplyContent.Replace("\n", "<br />")}
    </div>
    
    <p>Nếu bạn có bất kỳ câu hỏi nào thêm, vui lòng trả lời email này.</p>
    <p>Trân trọng,<br>Đội ngũ Hỗ trợ HolaBike</p>
</div>";

            var mailData = new MailData
            {
                EmailToId = contact.Email,
                EmailToName = contact.Email,
                EmailSubject = $"Re: {contact.Title}",
                EmailBody = emailHtmlBody // Gửi chuỗi HTML
            };

            try
            {
                bool emailSent = await _emailRepository.SendAsync(mailData);

                if (emailSent)
                {
                    contact.IsReplySent = true;
                    _contactRepository.Update(contact);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gửi email trả lời cho ContactId: {ContactId} thất bại.", contact.Id);
                // Cân nhắc ném lỗi ở đây để thông báo cho staff biết
                // throw new Exception("Lưu trả lời thành công, nhưng gửi email thất bại.");
            }

            return true;
        }
    }
}