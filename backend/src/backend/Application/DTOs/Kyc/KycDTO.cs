using Application.DTOs.BaseDTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Kyc
{
    public class KycDTO : BaseDTO<long>
    {
        [Required]
        public long UserId { get; set; }

        [StringLength(100)]
        public string? NumberCard { get; set; }

        [StringLength(255)]
        public string? IdFrontUrl { get; set; }

        [StringLength(255)]
        public string? IdBackUrl { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; }

        [Precision(0)]
        public DateTimeOffset? SubmittedAt { get; set; }
    }
}
