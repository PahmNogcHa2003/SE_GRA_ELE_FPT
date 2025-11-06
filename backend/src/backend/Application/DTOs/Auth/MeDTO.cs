using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Auth
{
    public class MeDTO
    {
        public long UserId { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? Dob { get; set; }
        public string? Gender { get; set; }
        public string? AddressDetail { get; set; }
        public string[] Roles { get; set; } = Array.Empty<string>();
        public decimal? WalletBalance { get; set; }
    }
}
