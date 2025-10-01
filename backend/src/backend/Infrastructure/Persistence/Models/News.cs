using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Models;

public partial class News
{
    [Key]
    public long Id { get; set; }

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
