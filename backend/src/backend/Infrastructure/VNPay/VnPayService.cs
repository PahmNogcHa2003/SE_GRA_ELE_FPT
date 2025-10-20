using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.User.Service;
using Infrastructure.Setting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using static Application.Interfaces.User.Service.IPaymentGatewayService;

namespace Infrastructure.VNPay
{
    // Infrastructure/VNPay/VnPayService.cs
    public class VnPayService : IPaymentGatewayService
    {
        private readonly VnPaySettings _opt;

        public VnPayService(IOptions<VnPaySettings> vnPayOptions)
        {
            _opt = vnPayOptions.Value;
        }

        public string CreatePaymentUrl(PaymentInfo p)
        {
            var txnRef = $"EcoJourneyHola_{p.OrderId}_{DateTime.Now:yyyyMMddHHmmss}";
            var dict = new SortedDictionary<string, string>(StringComparer.Ordinal)
            {
                ["vnp_Version"] = _opt.ApiVersion, // Lấy từ config
                ["vnp_Command"] = "pay",
                ["vnp_TmnCode"] = _opt.TmnCode,
                ["vnp_Amount"] = ((long)(p.Amount * 100)).ToString(),
                ["vnp_CreateDate"] = DateTime.Now.ToString("yyyyMMddHHmmss"),
                ["vnp_CurrCode"] = "VND",
                ["vnp_IpAddr"] = ToIPv4OrLoopback(p.IpAddress),
                ["vnp_Locale"] = "vn",
                ["vnp_OrderInfo"] = p.OrderInfo,
                ["vnp_OrderType"] = "other",
                ["vnp_ReturnUrl"] = _opt.ReturnUrl,
                ["vnp_TxnRef"] = txnRef
            };

            var rawForSign = VNPayHelper.BuildRawForSignNoEncode(dict);
            var secureHash = VNPayHelper.HmacSHA512(_opt.HashSecret, rawForSign);

            var queryEncoded = VNPayHelper.BuildQueryEncoded(dict);

            return $"{_opt.Url}?{queryEncoded}&vnp_SecureHashType=HMACSHA512&vnp_SecureHash={secureHash}";
        }

        public bool ValidateSignature(IQueryCollection collections)
        {
            var vnp_SecureHash = collections["vnp_SecureHash"].FirstOrDefault();
            if (string.IsNullOrEmpty(vnp_SecureHash)) return false;

            var dict = new SortedDictionary<string, string>(StringComparer.Ordinal);
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key)
                    && key.StartsWith("vnp_")
                    && key != "vnp_SecureHash"
                    && key != "vnp_SecureHashType")
                {
                    dict.Add(key, value.ToString());
                }
            }
            var rawForSign = VNPayHelper.BuildQueryEncoded(dict);
            var calculatedHash = VNPayHelper.HmacSHA512(_opt.HashSecret, rawForSign);

            return calculatedHash.Equals(vnp_SecureHash, StringComparison.OrdinalIgnoreCase);
        }

        public bool ValidateSignature(IDictionary<string, string> query) =>
            ValidateSignature(new QueryCollection(query.ToDictionary(k => k.Key, v => new StringValues(v.Value))));


        private string ToIPv4OrLoopback(string? ip)
        {
            if (string.IsNullOrWhiteSpace(ip) || ip.Contains(':')) return "127.0.0.1";
            return ip.Trim();
        }
    }
}
