using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Voucher
{
    public class VoucherDTO
    {
        [Required]
        [StringLength(50)]
        public string Code { get; set; } = null!;

        // % hoặc số tiền giảm trực tiếp
        [Required]
        public bool IsPercentage { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Value { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaxDiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinOrderAmount { get; set; }

        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }

        // Tổng số lần voucher được dùng
        public int? UsageLimit { get; set; }

        // Mỗi user dùng tối đa bao nhiêu lần
        public int? UsagePerUser { get; set; }

        public string Status { get; set; }
    }
}
