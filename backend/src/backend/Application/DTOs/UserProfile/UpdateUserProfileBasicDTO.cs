using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.UserProfile
{
    public class UpdateUserProfileBasicDTO
    {
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public string EmergencyName { get; set; } = default!;
        public string EmergencyPhone { get; set; } = default!;
        public string? AddressDetail { get; set; }
    }
}
