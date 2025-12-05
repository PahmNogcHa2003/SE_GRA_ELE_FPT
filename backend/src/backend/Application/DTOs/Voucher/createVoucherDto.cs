using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Voucher
{
    public class createVoucherDto
    {
        public long VoucherId { get; set; }
        public long UserId { get; set; }
        public long? OrderId { get; set; }
        public DateTimeOffset UsedAt { get; set; }
    }
}
