using System.Security.Claims;
using System.Text.Json;
using BarcodeDecodeFrontend.Data.Services.Interfaces;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace BarcodeDecodeFrontend.Data.Services.Auth;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ITokenProvider _tokenProvider;
    private readonly ILocalStorageService _localStorage;
    private readonly ILogger<JwtAuthenticationStateProvider> _logger;

    public JwtAuthenticationStateProvider(ILogger<JwtAuthenticationStateProvider> logger, ITokenProvider tokenProvider, ILocalStorageService localStorage)
    {
        _logger = logger;
        _tokenProvider = tokenProvider;
        _localStorage = localStorage;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = new ClaimsPrincipal();
        try
        {
            var token = _tokenProvider.Token;
            ClaimsIdentity identity;

            identity = string.IsNullOrWhiteSpace(token)
                ? new ClaimsIdentity()
                : new ClaimsIdentity(ParseClaims(token), "jwt");

            user = new ClaimsPrincipal(identity);
        }
        catch(InvalidOperationException)
        {
            _logger.LogWarning("InvalidOperationException occured in JwtAuthenticationStateProvider.");
        }
        return Task.FromResult(new AuthenticationState(user));
    }

    public void MarkUserAsAuthenticated(string token)
    {
        _tokenProvider.Token = token;
        _ =_localStorage.SetItemAsStringAsync("authToken", token);
        var identity = new ClaimsIdentity(ParseClaims(token), "jwt");
        NotifyAuthenticationStateChanged(Task.FromResult(
            new AuthenticationState(new ClaimsPrincipal(identity))));
    }

    public void MarkUserAsLoggedOut()
    {
        _tokenProvider.Token = null;
        NotifyAuthenticationStateChanged(Task.FromResult(
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
    }

    private IEnumerable<Claim> ParseClaims(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var json = DecodeBase64(payload);
        var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json)!;
        return dict
            .Where(kv => kv.Key != "exp" && kv.Key != "nbf")
            .Select(kv => new Claim(kv.Key, kv.Value.ToString()!));
    }

    private static string DecodeBase64(string str)
    {
        str = str.PadRight(str.Length + (4 - str.Length % 4) % 4, '=');
        var bytes = Convert.FromBase64String(str);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }
}