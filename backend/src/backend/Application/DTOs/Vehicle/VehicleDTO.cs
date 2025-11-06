using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Vehicle
{
    public class VehicleDTO : BaseDTO<long>
    {
        public long? CategoryId { get; set; }

        [StringLength(50)]
        public string BikeCode { get; set; } = null!;

        public int? BatteryLevel { get; set; }

        public bool? ChargingStatus { get; set; }

        [StringLength(20)]
        [Unicode(false)]
        public string Status { get; set; } = null!;

        public long? StationId { get; set; }

        [Precision(0)]
        public DateTimeOffset CreatedAt { get; set; }

    }
}
