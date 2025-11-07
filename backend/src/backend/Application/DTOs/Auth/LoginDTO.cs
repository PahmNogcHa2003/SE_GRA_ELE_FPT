using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string DeviceId { get; set; }

        [Required]
        [StringLength(1024)]
        public string PushToken { get; set; }

        [Required]
        [StringLength(50)]
        public string Platform { get; set; }
    }
}
