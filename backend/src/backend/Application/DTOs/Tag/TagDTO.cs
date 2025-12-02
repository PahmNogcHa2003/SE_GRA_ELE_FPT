using Application.DTOs.BaseDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Tag
{
    public class TagDTO : BaseDTO<long>
    {
        [StringLength(50)]
        public string Name { get; set; } = null!;

    }
}
