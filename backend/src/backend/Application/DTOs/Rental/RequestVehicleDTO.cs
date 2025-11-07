using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Rental
{
    public class RequestVehicleDTO
    {
        [Required]
        public long VehicleId { get; set; }

        [Required(ErrorMessage = "Current latitude is required.")]
        [Range(-90.0, 90.0, ErrorMessage = "Invalid latitude value.")]
        public double CurrentLatitude { get; set; }

        [Required(ErrorMessage = "Current longitude is required.")]
        [Range(-180.0, 180.0, ErrorMessage = "Invalid longitude value.")]
        public double CurrentLongitude { get; set; }
    }
}
