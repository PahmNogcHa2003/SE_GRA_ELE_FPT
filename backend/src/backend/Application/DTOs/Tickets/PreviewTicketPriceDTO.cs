using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Tickets
{
    public class PreviewTicketPriceDTO
    {
        public decimal Subtotal { get; set; }      // Giá gốc
        public decimal Discount { get; set; }      // Số tiền giảm
        public decimal Total { get; set; }         // Giá phải trả
        public string? VoucherMessage { get; set; } // Thông tin thêm (optional)
    }
}
