using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BarcodeDecodeBackend.Services.Interfaces;
using BarcodeDecodeLib.Models.Dtos.Configs;
using BarcodeDecodeLib.Models.Dtos.Messages.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BarcodeDecodeBackend.Services.Processing;

public class AuthMessageHandler : IAuthMessageHandler
{
    private readonly UserManager<IdentityUser> _userMgr;
    private readonly JwtSettings _jwtSettings;

    public AuthMessageHandler(UserManager<IdentityUser> userMgr, IOptions<JwtSettings> jwtSettings)
    {
        _userMgr = userMgr;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResult?> Login(LoginDto dto)
    {
        var user = await _userMgr.FindByNameAsync(dto.Username);
        if (user == null || !await _userMgr.CheckPasswordAsync(user, dto.Password))
            return null;
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var roles = await _userMgr.GetRolesAsync(user);
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var expirationTime = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes);
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expirationTime,
            signingCredentials: creds
        );

        var result = new LoginResult()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            TokenExpiration = expirationTime
        };

        return result;
    }
}