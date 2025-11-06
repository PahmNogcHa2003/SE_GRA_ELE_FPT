using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.DTOs
{
    public class NewsDTO : BaseDTO<long>
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
        public List<long> TagIds { get; set; } = new List<long>();
        public List<string>? TagNames { get; set; } 

    }
}
