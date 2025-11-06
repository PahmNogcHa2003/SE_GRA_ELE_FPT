using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Kyc
{
    public class CreateKycRequestDTO 
    {
        [Required]
        public string JsonData { get; set; }
        [Required]
        public IFormFile FrontImage { get; set; }

        [Required]
        public IFormFile BackImage { get; set; }

    }
}
