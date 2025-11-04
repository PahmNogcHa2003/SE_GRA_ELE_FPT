using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.User.Service
{
    public interface IPaymentGatewayService
    {
        string CreatePaymentUrl(PaymentInfo paymentInfo);
        bool ValidateSignature(IQueryCollection collections);
        bool ValidateSignature(IDictionary<string, string> query);
        public class PaymentInfo
        {
            public long OrderId { get; set; }
            public decimal Amount { get; set; }
            public string OrderInfo { get; set; } = string.Empty;
            public string IpAddress { get; set; } = string.Empty;
        }
    }
}
