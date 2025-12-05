using Application.DTOs.Voucher;
using Application.Interfaces.Base;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Staff.Service
{
    public interface IVoucherUsageService : IService<VoucherUsage, createVoucherUsageDto, long>,
          IService3DTO<VoucherUsage, createVoucherUsageDto, long>
    {
    }
}
