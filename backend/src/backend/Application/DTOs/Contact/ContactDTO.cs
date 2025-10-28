using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Application.DTOs.Contact
{
    public class ContactDTO : BaseDTO<long>
    {
        [StringLength(255)]
        public string? Email { get; set; }

        [StringLength(50)]
        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Open";

        public long? ReplyById { get; set; }

        [Required]
        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? ClosedAt { get; set; }
    }
}
