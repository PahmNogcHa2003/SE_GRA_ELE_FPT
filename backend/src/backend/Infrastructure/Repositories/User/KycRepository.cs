using Application.Interfaces.User.Repository;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.User
{
    public class KycRepository : BaseRepository<KycForm, long>, IKycRepository
    {
        public KycRepository(HolaBikeContext dbContext) : base(dbContext)
        {
        }
        public async Task<bool> IsVerifiedAsync(long userId, CancellationToken ct)
        {
            var kyc = await _dbContext.KycForms
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.SubmittedAt)
                .FirstOrDefaultAsync(ct);

            if (kyc is null)
                return false;

            return string.Equals(kyc.Status, "Approve", StringComparison.OrdinalIgnoreCase);
        }

    }
}
