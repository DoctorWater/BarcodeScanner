using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BarcodeDecodeBackend.Services.Interfaces;
using BarcodeDecodeLib.Models.Dtos.Messages.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BarcodeDecodeBackend.Services.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private IAuthMessageHandler _authMessageHandler;

    public AuthController(IAuthMessageHandler authMessageHandler)
    {
        _authMessageHandler = authMessageHandler;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await _authMessageHandler.Login(dto);
        if (token == null) return Unauthorized("Invalid username or password");
        var result = new LoginResult { Token = token };
        return Ok(result);
    }
}