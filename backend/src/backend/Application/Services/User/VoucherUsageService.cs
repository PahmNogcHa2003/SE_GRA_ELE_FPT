using Application.DTOs.Voucher;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.Staff.Repository;
using Application.Interfaces.Staff.Service;
using Application.Services.Base;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.User
{
    public class VoucherUsageService : GenericService<VoucherUsage, createVoucherUsageDto, long>, IVoucherUsageService
    {
        public VoucherUsageService(IRepository<VoucherUsage, long> repo, IMapper mapper, IUnitOfWork uow) : base(repo, mapper, uow)
        {
        }
    }
}
