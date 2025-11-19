using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("Contact")]
public class Contact : BaseEntity<long>
{
    [StringLength(255)]
    public string? Email { get; set; }

    [StringLength(50)]
    public string? PhoneNumber { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(4000)]
    public string Content { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = ContactStatus.Open;
    public long? ReplyById { get; set; }

    [StringLength(4000)]
    public string? ReplyContent { get; set; }

    [Precision(0)]
    public DateTimeOffset? ReplyAt { get; set; }

    public bool IsReplySent { get; set; } = false;

    [Required]
    [Precision(0)]
    public DateTimeOffset CreatedAt { get; set; }

    [Precision(0)]
    public DateTimeOffset? ClosedAt { get; set; }

    [ForeignKey(nameof(ReplyById))]
    public AspNetUser? Reply { get; set; }

    // Hằng số trạng thái (Giả định chúng nằm trong Domain.Enums.ContactStatus như bạn đã dùng trong Factory)
    public const string StatusOpen = "Open";
    public const string StatusReplied = "Replied";
    public const string StatusClosed = "Closed";

    // Constructor bắt buộc để tạo Contact mới (Giả định)
    public Contact(string title, string content, DateTimeOffset createdAt)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Title and Content are required.");
        }

        Title = title;
        Content = content;
        CreatedAt = createdAt;
        Status = StatusOpen; // Mặc định là Open
    }

    // Constructor mặc định cho EF Core
    public Contact() { }

    // 1. Phản hồi liên hệ (Chỉ nhân viên mới làm được)
    public void SubmitReply(long replyById, string replyContent, DateTimeOffset replyAt)
    {
        if (Status == StatusClosed)
        {
            throw new InvalidOperationException("Cannot reply to a closed contact.");
        }
        if (string.IsNullOrWhiteSpace(replyContent))
        {
            throw new ArgumentException("Reply content cannot be empty.", nameof(replyContent));
        }

        ReplyById = replyById;
        ReplyContent = replyContent;
        ReplyAt = replyAt;
        Status = StatusReplied; // Chuyển sang trạng thái đã phản hồi (nhưng chưa chắc đã gửi)
    }

    // 2. Đánh dấu là đã gửi
    public void MarkAsSent()
    {
        if (Status != StatusReplied)
        {
            throw new InvalidOperationException("Only 'Replied' contacts can be marked as sent.");
        }
        IsReplySent = true;
    }

    // 3. Đóng liên hệ
    public void Close(DateTimeOffset closedAt)
    {
        if (Status == StatusClosed) return;

        Status = StatusClosed;
        ClosedAt = closedAt;
    }
}
