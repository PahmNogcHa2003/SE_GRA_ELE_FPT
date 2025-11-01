using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Contact
{
    public class CreateContactDTO 
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
    }
}
