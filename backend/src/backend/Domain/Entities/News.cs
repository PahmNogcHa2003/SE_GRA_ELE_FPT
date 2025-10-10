using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Microsoft.EntityFrameworkCore.Index(nameof(AuthorId), Name = "IX_News_AuthorId")]
public partial class News : BaseEntity<long>
{
    [StringLength(200)]
    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public long AuthorId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public bool IsActive { get; set; }

    [ForeignKey("AuthorId")]
    [InverseProperty("News")]
    public virtual AspNetUser Author { get; set; } = null!;

    [ForeignKey("NewsId")]
    [InverseProperty("News")]
    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
