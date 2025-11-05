using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class FullAddress
    {
        public string? ProvinceCode { get; set; }
        public string? ProvinceName { get; set; }

        public string? WardCode { get; set; }
        public string? WardName { get; set; }

        // Thông tin hiển thị đầy đủ
        public string? FullName
        {
            get
            {
                var parts = new List<string>();
                if (!string.IsNullOrEmpty(WardName)) parts.Add(WardName);
                if (!string.IsNullOrEmpty(ProvinceName)) parts.Add(ProvinceName);
                return string.Join(", ", parts);
            }
        }
    }
}
