using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.User.Service;
using Application.Services.Base;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.User
{
    public class UserWalletService : GenericService<Wallet, WalletDTO, long>, IUserWalletService
    {
        public UserWalletService(IRepository<Wallet,long> repo,IUnitOfWork uow, IMapper mapper)
            : base(repo, mapper, uow)
        {
        }

        // TRIỂN KHAI PHƯƠNG THỨC MỚI
        public async Task<WalletDTO?> GetByUserIdAsync(long? userId, CancellationToken ct = default)
        {
            var walletEntity = await _repo.Query()
            .AsNoTracking()
                .FirstOrDefaultAsync(w => w.UserId == userId, ct);

            return walletEntity == null ? null : _mapper.Map<WalletDTO>(walletEntity);
        }
    }
}
