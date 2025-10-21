using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.Base;
using Domain.Entities;

namespace Application.Interfaces.User.Service
{
        public interface IUserWalletService : IService<Wallet, WalletDTO, long>
        {
            Task<WalletDTO?> GetByUserIdAsync(long userId, CancellationToken ct = default);
   
    }
}
