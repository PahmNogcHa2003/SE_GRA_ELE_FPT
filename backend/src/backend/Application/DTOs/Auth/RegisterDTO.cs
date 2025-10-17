using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Auth
{
    public class RegisterDTO
    {
        [Required]
        [StringLength(15, MinimumLength = 9)]
        public string PhoneNumber { get; set; } // ĐÃ SỬA: string

        [Required]
        public string FullName { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 9)]
        public string IdentityNumber { get; set; } // ĐÃ SỬA: string

        [Required]
        public int ProvinceId { get; set; }

        [Required]
        public string ProvinceName { get; set; }

        [Required]
        public int WardId { get; set; }

        [Required]
        public string WardName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // Mặc định là Khác, nhưng vẫn yêu cầu (Required) để validation
        [Required]
        public string? Gender { get; set; } = "Khác";

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password))] // Thêm Compare validation để kiểm tra trùng khớp
        public string ConfirmPassword { get; set; }
    }
}