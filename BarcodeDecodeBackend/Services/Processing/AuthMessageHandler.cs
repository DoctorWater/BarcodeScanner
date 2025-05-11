using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BarcodeDecodeBackend.Services.Interfaces;
using BarcodeDecodeLib.Models.Dtos.Messages.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BarcodeDecodeBackend.Services.Processing;

public class AuthMessageHandler : IAuthMessageHandler
{
    private readonly UserManager<IdentityUser> _userMgr;
    private readonly IConfiguration _config;

    public AuthMessageHandler(UserManager<IdentityUser> userMgr, IConfiguration config)
    {
        _userMgr = userMgr;
        _config = config;
    }

    public async Task<string?> Login(LoginDto dto)
    {
        var user = await _userMgr.FindByNameAsync(dto.Username);
        if (user == null || !await _userMgr.CheckPasswordAsync(user, dto.Password))
            return null;

        var jwt = _config.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var roles = await _userMgr.GetRolesAsync(user);
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwt["DurationInMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}