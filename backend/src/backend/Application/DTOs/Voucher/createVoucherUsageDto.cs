using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Voucher
{
    public class createVoucherUsageDto
    {
        [Required]
        public long VehicleId { get; set; }

        public long? UserId { get; set; }

        [StringLength(50)]
        public string? ChangeType { get; set; }

        [StringLength(255)]
        public string? Note { get; set; }
    }
}
