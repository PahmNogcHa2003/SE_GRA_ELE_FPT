using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace  Application.Services.Identity
{
    public static class ClaimsPrincipalExtensions
    {
        public static long? GetUserIdAsLong(this ClaimsPrincipal user)
        {
            var idStr = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(idStr, out var id) ? id : (long?)null;
        }
    }
}
