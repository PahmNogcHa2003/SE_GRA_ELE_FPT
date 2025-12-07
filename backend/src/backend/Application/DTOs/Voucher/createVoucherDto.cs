using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Voucher
{
    public class createVoucherDto
    {
        [Required]
        [StringLength(50)]
        public string Code { get; set; } = null!;

        [Required]
        public bool IsPercentage { get; set; }

        [Required]
        public decimal Value { get; set; }

        public decimal? MaxDiscountAmount { get; set; }

        public decimal? MinOrderAmount { get; set; }

        [Required]
        public DateTimeOffset StartDate { get; set; }

        [Required]
        public DateTimeOffset EndDate { get; set; }

        public int? UsageLimit { get; set; }
        public int? UsagePerUser { get; set; }

        public string? Status { get; set; }
    }
    public class UpdateVoucherDTO : createVoucherDto
    {

    }
}
