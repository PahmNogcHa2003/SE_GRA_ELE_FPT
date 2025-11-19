using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("News")]
[Index(nameof(Slug), Name = "UQ_News_Slug", IsUnique = true)]
public partial class News : BaseEntity<long>
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = null!;

    [Required]
    [StringLength(200)]
    public string Slug { get; set; } = null!;

    [StringLength(255)]
    public string? Banner { get; set; }

    public string? Content { get; set; }

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = "Draft";

    [Precision(0)]
    public DateTimeOffset? PublishedAt { get; set; }

    public long? PublishedBy { get; set; }

    [Precision(0)]
    public DateTimeOffset? ScheduledAt { get; set; }

    [Required]
    [Precision(0)]
    public DateTimeOffset CreatedAt { get; set; }

    [Required]
    public long UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    // SỬA 1: Trỏ đến collection AuthoredNews trong AspNetUser
    [InverseProperty(nameof(AspNetUser.AuthoredNews))]
    public virtual AspNetUser User { get; set; } = null!;

    [ForeignKey(nameof(PublishedBy))]
    // SỬA 2: Trỏ đến collection PublishedNews trong AspNetUser
    [InverseProperty(nameof(AspNetUser.PublishedNews))]
    public virtual AspNetUser? Publisher { get; set; }

    [InverseProperty("News")]
    public virtual ICollection<TagNew> TagNews { get; set; } = new List<TagNew>();

    public const string StatusDraft = "Draft";
    public const string StatusScheduled = "Scheduled";
    public const string StatusPublished = "Published";

    // 1. Đặt lịch công bố
    public void Schedule(DateTimeOffset scheduleTime, long userId)
    {
        if (scheduleTime <= CreatedAt)
        {
            throw new ArgumentException("Schedule time must be after creation time.", nameof(scheduleTime));
        }

        Status = StatusScheduled;
        ScheduledAt = scheduleTime;
        PublishedAt = null;
        PublishedBy = userId; // Ghi nhận người đặt lịch
    }

    // 2. Công bố bài viết
    public void Publish(DateTimeOffset publishTime, long userId)
    {
        if (publishTime < CreatedAt)
        {
            throw new ArgumentException("Publish time cannot be before creation time.", nameof(publishTime));
        }

        // Logic: Bất kỳ bài viết nào cũng có thể được công bố.
        Status = StatusPublished;
        PublishedAt = publishTime;
        PublishedBy = userId;
        ScheduledAt = null; // Nếu đã công bố, lịch trình sẽ bị hủy bỏ/ghi đè
    }

    // 3. Đưa bài viết trở lại trạng thái nháp
    public void Draft()
    {
        if (Status == StatusPublished)
        {
            throw new InvalidOperationException("Cannot draft a published news article.");
        }
        Status = StatusDraft;
        PublishedAt = null;
        ScheduledAt = null;
    }
}

