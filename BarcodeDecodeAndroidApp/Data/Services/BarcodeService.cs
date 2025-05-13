using System.Net.Http.Json;
using BarcodeDecodeLib.Models.Dtos.Messages.Barcode;
using Newtonsoft.Json;

namespace MauiAndroid.App.Services;

public class BarcodeService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public BarcodeService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory
            ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public async Task<BarcodeResponseMessageBatch> SendBarcodeRequest(
        BarcodeRequestMessageBatch message,
        CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("AuthorizedClient");

        string rawHost = Preferences.Get("ServerAddress", "192.168.1.104")
            .Trim()
            .TrimEnd('/', '\\');
        rawHost = new string(rawHost
            .Trim('\uFEFF', '\u200B')
            .Where(c => !char.IsControl(c))
            .ToArray());

        string portStr = Preferences.Get("ServerPort", "21101").Trim();
        int port = int.TryParse(portStr, out var p) ? p : 21101;

        client.BaseAddress = new Uri($"https://{rawHost}:{port}/");
        var response = await client
                    .PostAsJsonAsync("api/barcode/batch", message, cancellationToken);


        if (!response.IsSuccessStatusCode || response.Content == null)
        {
            throw new HttpRequestException(
                $"Ошибка при отправке запроса. Статус: {response.StatusCode}. Причина: {response.ReasonPhrase}");
        }

        return await response.Content
            .ReadFromJsonAsync<BarcodeResponseMessageBatch>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Пустой ответ от сервера.");
    }
}
