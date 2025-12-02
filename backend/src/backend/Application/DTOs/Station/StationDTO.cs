using Application.DTOs.BaseDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Station
{
    public class StationDTO : BaseDTO<long>
    {
        [StringLength(150)]
        public string? Name { get; set; }

        [StringLength(255)]
        public string? Location { get; set; }

        public int? Capacity { get; set; }

        [Column(TypeName = "decimal(9, 6)")]
        public decimal? Lat { get; set; }

        [Column(TypeName = "decimal(9, 6)")]
        public decimal? Lng { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(255)]
        public string? Image { get; set; }
        public int VehicleAvailable { get; set; }
        public double? DistanceKm { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
