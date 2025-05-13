using System.Net.Http.Json;
using System.Text.Json;
using BarcodeDecodeLib.Models.Dtos.Messages.Auth;
using MauiAndroid.App.Utils;

namespace MauiAndroid.App.Services;
public class AuthService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITokenProvider _tokenProvider;

    public AuthService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
    {
        _httpClientFactory = httpClientFactory;
        _tokenProvider = tokenProvider;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var client = _httpClientFactory.CreateClient("AnonymousClient");

        try
        {
            string rawAddress = Preferences.Get("ServerAddress", "192.168.1.104")
    .Trim()                      // обрезаем пробелы по краям
    .TrimEnd('/', '\\');         // убираем случайные слеши в конце

            rawAddress = rawAddress.Trim().Trim('\uFEFF', '\u200B');
            rawAddress = new string(rawAddress.Where(c => !char.IsControl(c)).ToArray());
            // 2) Выведем в лог каждый символ и длину
            Console.WriteLine($"[{rawAddress}] (Length = {rawAddress.Length})");
            for (int i = 0; i < rawAddress.Length; i++)
                Console.WriteLine($" char #{i} = '{rawAddress[i]}' (0x{(int)rawAddress[i]:X2})");

            // 3) Попробуем собрать базовый Uri через TryCreate
            if (!Uri.TryCreate(rawAddress, UriKind.Absolute, out var baseUri))
            {
                Console.WriteLine("❌ rawAddress can't be parsed as absolute URI");
            }
            else
            {
                Console.WriteLine($"✔ baseUri = {baseUri}");
            }

            // 4) Если rawAddress — это просто хост без схемы/порта, вручную добавляем схему
            if (!rawAddress.Contains("://"))
                rawAddress = "https://" + rawAddress;

            if (!Uri.TryCreate(rawAddress, UriKind.Absolute, out baseUri))
            {
                Console.WriteLine("❌ Even schema doesn't help");
            }
            else
            {
                // 5) Пробуем окончательную сборку с путем
                Uri testUri = new Uri(baseUri, "/api/auth/login");
                Console.WriteLine($"✔ loginUri = {testUri}");
            }

            string serverPortStr = Preferences.Get("ServerPort", "21101");
            int serverPort = int.TryParse(serverPortStr, out var p) ? p : 21101;

            var builder = new UriBuilder
            {
                Scheme = "https",
                Host = "192.168.1.106",
                Port = 21101,
                Path = "/api/auth/login"
            };
            Uri loginUri = builder.Uri;

            Console.WriteLine($"Login URI: {loginUri}");

            var dto = new LoginDto { Username = username, Password = password };
            HttpResponseMessage response = await client.PostAsJsonAsync(loginUri, dto);

            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadFromJsonAsync<LoginResult>(cancellationToken: default)!;

            if(result.Token is null || string.IsNullOrEmpty(result.Token))
                return false;
            string token = result.Token;
            await _tokenProvider.SaveTokenAsync(token);
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