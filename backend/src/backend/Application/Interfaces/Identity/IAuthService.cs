using Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Identity
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO model);
        Task<AuthResponseDTO> LoginAsync(LoginDTO model);
    }
}
