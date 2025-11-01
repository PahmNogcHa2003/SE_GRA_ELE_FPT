using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Payments
{
    public class PaymentResultDTO
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string RspCode { get; set; } = "99";
        public object? Order { get; set; }
        public object? Transaction { get; set; }
    }
}
