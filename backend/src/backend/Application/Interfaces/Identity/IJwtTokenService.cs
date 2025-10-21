using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Identity
{
    public interface IJwtTokenService
    {
        Task<string> GenerateJwtTokenAsync(AspNetUser user);
    }
}
