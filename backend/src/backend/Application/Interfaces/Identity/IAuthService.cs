using Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Identity
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO model, CancellationToken ct = default);
        Task<AuthResponseDTO> LoginAsync(LoginDTO model, CancellationToken ct = default);
        Task<MeDTO?> GetMeAsync(ClaimsPrincipal userPrincipal, CancellationToken ct = default);
        Task<AuthResponseDTO> ChangePasswordAsync(ClaimsPrincipal userPrincipal, ChangePasswordDTO model, CancellationToken ct = default);
    }
}
