using Application.DTOs.BaseDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CategoriesVehicle
{
    public class CategoriesVehicleDTO : BaseDTO<long>
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}
