using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class NewsDTO : BaseDTO<long>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public long AuthorId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public bool IsActive { get; set; }

        // Thêm thuộc tính này vào
        public List<long> TagIds { get; set; } = new List<long>();

    }
}
