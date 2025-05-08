using BarcodeDecodeFrontend.Data.Services.Messaging;
using BarcodeDecodeLib.Models.Dtos.Messages.Auth;
using Microsoft.AspNetCore.Components.Authorization;

namespace BarcodeDecodeFrontend.Data.Services.Auth;

public class AuthService
{
    private readonly JwtAuthenticationStateProvider _authState;
    private readonly IHttpMessagePublisher _httpMessagePublisher;

    public AuthService(AuthenticationStateProvider authState, IHttpMessagePublisher httpMessagePublisher)
    {
        _httpMessagePublisher = httpMessagePublisher;
        _authState = (JwtAuthenticationStateProvider)authState;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var result = await _httpMessagePublisher.SendLoginMessage(new LoginDto(){Username = username, Password = password});
        if (result == null) return false;
        _authState.MarkUserAsAuthenticated(result.Token);
        return true;
    }

    public void LogoutAsync()
    {
        _authState.MarkUserAsLoggedOut();
    }
}