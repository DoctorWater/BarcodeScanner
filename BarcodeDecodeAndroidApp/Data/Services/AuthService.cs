using System.Net.Http.Json;
using BarcodeDecodeLib.Models.Dtos.Messages.Auth;
using MauiAndroid.App.Data.Utils;

namespace MauiAndroid.App.Data.Services;
public class AuthService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITokenProvider _tokenProvider;

    public AuthService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
    {
        _httpClientFactory = httpClientFactory;
        _tokenProvider = tokenProvider;
    }
    
    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient("AnonymousClient");
        var baseHost = Preferences.Get("ServerAddress", "192.168.1.104");
        var basePort = Preferences.Get("ServerPort", "21101");
        var uri = new Uri($"https://{baseHost}:{basePort}");
        client.BaseAddress = uri;
        client.Timeout = TimeSpan.FromSeconds(30);
        return client;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        using var client = CreateClient();

        try
        {
            var dto = new LoginDto { Username = username, Password = password };
            HttpResponseMessage response = await client.PostAsJsonAsync("api/auth/login", dto);

            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadFromJsonAsync<LoginResult>(cancellationToken: default)!;

            if(result.Token is null || string.IsNullOrEmpty(result.Token))
                return false;
            string token = result.Token;
            await _tokenProvider.SaveTokenAsync(token, result.TokenExpiration);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return false;
    }

    public async Task<string?> GetSecuredDataAsync()
    {
        var client = _httpClientFactory.CreateClient("AuthorizedClient");
        var response = await client.GetAsync("/api/protected");

        return response.IsSuccessStatusCode
            ? await response.Content.ReadAsStringAsync()
            : null;
    }
}