using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.BaseDTO;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.DTOs.New
{
    public class NewsDTO : BaseDTO<long>
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        public string Slug { get; set; } = null!;

        [StringLength(255)]
        public string? Banner { get; set; }

        public string? Content { get; set; }

        [Required]
        [StringLength(20)]
        [Unicode(false)]
        public string Status { get; set; } 

        [Precision(0)]
        public DateTimeOffset? ScheduledAt { get; set; }
        public List<long> TagIds { get; set; }

    }
}
