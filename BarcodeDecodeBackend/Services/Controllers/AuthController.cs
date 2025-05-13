using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BarcodeDecodeBackend.Services.Interfaces;
using BarcodeDecodeBackend.Services.Processing;
using BarcodeDecodeLib.Models.Dtos.Messages.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Prometheus;

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
        using var timer = MetricsRegistry.AuthLoginDuration.NewTimer();

        MetricsRegistry.AuthLoginAttemptsTotal.Inc();
        var token = await _authMessageHandler.Login(dto);
        var result = new LoginResult{ Token = token };

        if (result.Token is null)
        {
            MetricsRegistry.AuthLoginFailureTotal.Inc();
            return Unauthorized("Invalid username or password");
        }

        MetricsRegistry.AuthLoginSuccessTotal.Inc();
        return Ok(result);
    }
}