using Application.DTOs.BaseDTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.UserDevice
{
    public class UserDeviceDTO : BaseDTO<long>
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public Guid DeviceId { get; set; }

        [StringLength(50)]
        public string? Platform { get; set; }

        [StringLength(1024)]
        public string? PushToken { get; set; }

        [Precision(0)]
        public DateTimeOffset? LastLoginAt { get; set; }

        [Required]
        [Precision(0)]
        public DateTimeOffset CreatedAt { get; set; }

        [Required]
        [Precision(0)]
        public DateTimeOffset UpdatedAt { get; set; }

    }
}
