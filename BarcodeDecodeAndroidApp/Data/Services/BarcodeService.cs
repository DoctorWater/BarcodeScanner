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
        _httpClient = new HttpClient();

        string serverAddress = Preferences.Get("ServerAddress", "192.168.1.104");
        string serverPort = Preferences.Get("ServerPort", "7214");

        _baseUrl = $"http://{serverAddress}:{serverPort}/api/barcode/info";
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
