using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.BaseDTO
{
    public abstract class BaseDTO<TKey>
    {
        public TKey Id { get; set; } = default!;
    }
}
