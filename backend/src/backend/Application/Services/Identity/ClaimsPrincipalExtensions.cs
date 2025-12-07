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
        public static long GetUserIdAsLong(this ClaimsPrincipal user)
        {
            var idStr = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (long.TryParse(idStr, out var id))
            {
                return id;
            }
            throw new UnauthorizedAccessException("Không tìm thấy User ID trong Token.");
        }
    }
}
