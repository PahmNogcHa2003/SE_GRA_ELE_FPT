using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Payments
{
    public class CreatePaymentRequestDTO
    {

        [Required]
        [Range(10000, 100000000, ErrorMessage = "Số tiền phải từ 10,000 VND đến 100,000,000 VND")]
        public decimal Amount { get; set; }
    }
}
