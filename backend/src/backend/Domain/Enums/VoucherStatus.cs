using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public class VoucherStatus
    {
        public const string Inactive = "Inactive";   // Chưa kích hoạt
        public const string Active = "Active";     // Đang hoạt động
        public const string Expired = "Expired";    // Hết hạn
        public const string Disabled = "Disabled";   // Bị tắt
    }
}
