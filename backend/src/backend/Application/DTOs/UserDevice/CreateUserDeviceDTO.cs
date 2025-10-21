using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.UserDevice
{
    public class CreateUserDeviceDTO : BaseDTO<long>
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public Guid DeviceId { get; set; }

        [Required]
        [StringLength(1024)]
        public string PushToken { get; set; }

        [Required]
        [StringLength(50)]
        public string Platform { get; set; }
    }
}
