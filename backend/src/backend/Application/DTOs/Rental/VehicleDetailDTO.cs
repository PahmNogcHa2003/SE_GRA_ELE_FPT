using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Rental
{
    public class VehicleDetailDTO
    {
        [StringLength(50)]
        public string BikeCode { get; set; } 

        public string CategoryName { get; set; }

        public string VehicleStatus { get; set; }

        public string StationName { get; set; }
    }
}
