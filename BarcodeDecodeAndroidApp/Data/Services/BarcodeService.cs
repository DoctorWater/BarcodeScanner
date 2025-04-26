using System.Net.Http.Json;
using BarcodeDecodeLib.Models.Dtos.Messages.Barcode;
using Newtonsoft.Json;

namespace MauiAndroid.App;

public class BarcodeService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public BarcodeService()
    {
        //УБРАТЬ! СДЕЛАТЬ НОРМАЛЬНЫЙ СЕРТИФИКАТ!!!
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        _httpClient = new HttpClient(handler);

        string serverAddress = Preferences.Get("ServerAddress", "192.168.1.104");
        string serverPort = Preferences.Get("ServerPort", "21101");

        _baseUrl = $"https://{serverAddress}:{serverPort}";
        _httpClient.BaseAddress = new Uri(_baseUrl);
    }

    public async Task<BarcodeResponseMessageBatch> SendBarcodeRequest(
            BarcodeRequestMessageBatch message,
            CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/barcode/batch", message, cancellationToken);
        if (!response.IsSuccessStatusCode || response.Content == null)
        {
            throw new HttpRequestException(
                $"Ошибка при отправке запроса. Статус: {response.StatusCode}. Причина: {response.ReasonPhrase}");
        }
        return await response.Content.ReadFromJsonAsync<BarcodeResponseMessageBatch>(cancellationToken: cancellationToken)!;
    }
}
