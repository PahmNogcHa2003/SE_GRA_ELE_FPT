using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.VNPay
{
    public static class VNPayHelper
    {
        // Encode UTF-8 cho GHÉP URL
        public static string UrlEncodeUtf8(string? s)
            => System.Web.HttpUtility.UrlEncode(s ?? string.Empty, Encoding.UTF8) ?? string.Empty;

        // Chuỗi KÝ kiểu (2): KHÔNG encode value, chỉ sort & join
        public static string BuildRawForSignNoEncode(IDictionary<string, string> dict)
        {
            return string.Join("&",
                dict.OrderBy(k => k.Key, StringComparer.Ordinal)
                    .Select(kv => $"{kv.Key}={kv.Value ?? string.Empty}"));
        }

        // Chuỗi GHÉP URL: encode value
        public static string BuildQueryEncoded(IDictionary<string, string> dict)
        {
            return string.Join("&",
                dict.OrderBy(k => k.Key, StringComparer.Ordinal)
                    .Select(kv => $"{kv.Key}={UrlEncodeUtf8(kv.Value)}"));
        }

        public static string HmacSHA512(string key, string data)
        {
            using var h = new HMACSHA512(Encoding.UTF8.GetBytes((key ?? "").Trim()));
            var hash = h.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToHexString(hash); // UPPERCASE (OK), so sánh ignore-case
        }
    }
}
