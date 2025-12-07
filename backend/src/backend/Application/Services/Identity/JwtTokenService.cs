using Application.Interfaces.Identity;
using Domain.Entities;
using Microsoft.AspNetCore.Identity; // Thêm using cho UserManager
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Identity
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        // 1. Inject UserManager để có thể truy vấn Roles
        private readonly UserManager<AspNetUser> _userManager;

        public JwtTokenService(IConfiguration configuration, UserManager<AspNetUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<string> GenerateJwtTokenAsync(AspNetUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),               
            };

            
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var keyString = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing in configuration.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(20), 
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
