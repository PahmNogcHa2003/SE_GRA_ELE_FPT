using Application.DTOs.Contact;
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
    public interface IVoucherService : IService<Voucher, VoucherDTO, long>, IService3DTO<Voucher, VoucherDTO, long>
    {
    }
}
