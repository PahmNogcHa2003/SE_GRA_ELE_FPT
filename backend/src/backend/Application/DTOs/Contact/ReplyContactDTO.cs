using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Contact
{
    public class ReplyContactDTO
    {
        [StringLength(4000)]
        public string? ReplyContent { get; set; }
    }
}
