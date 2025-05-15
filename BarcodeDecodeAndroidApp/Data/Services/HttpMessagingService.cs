using System.Net;
using System.Net.Http.Json;
using BarcodeDecodeLib.Models.Dtos.Messages.Barcode;
using BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;
using BarcodeDecodeLib.Models.Dtos.Messages.Tsu;

namespace MauiAndroid.App.Data.Services;

public class HttpMessagingService : IHttpMessagingService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HttpMessagingService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient("AuthorizedClient");
        var baseHost = Preferences.Get("ServerAddress", "192.168.1.104");
        var basePort = Preferences.Get("ServerPort", "21101");
        var uri = new Uri($"https://{baseHost}:{basePort}");
        client.BaseAddress = uri;
        client.Timeout = TimeSpan.FromSeconds(30);
        return client;
    }

    public async Task<BarcodeResponseMessageBatch> SendBarcodeRequest(
        BarcodeRequestMessageBatch message,
        CancellationToken cancellationToken = default)
    {
        using var client = CreateClient();
        var response = await client.PostAsJsonAsync("api/barcode/batch", message, cancellationToken);
        if (response.StatusCode is HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException(
                "Авторизация не прошла. Проверьте данные авторизации.");
        }

        
        if (!response.IsSuccessStatusCode || response.Content == null)
        {
            throw new HttpRequestException(
                $"Ошибка при отправке запроса. Статус: {response.StatusCode}. Причина: {response.ReasonPhrase}");
        }

        return await response.Content.ReadFromJsonAsync<BarcodeResponseMessageBatch>(
            cancellationToken: cancellationToken)!;
    }

    public async Task<TsuResponseMessage> SendTsuChangeMessage(
        TsuChangeMessage message,
        CancellationToken cancellationToken = default)
    {
        using var client = CreateClient();
        var response = await client.PostAsJsonAsync("api/tsu/change", message, cancellationToken);
        if (response.StatusCode is HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException(
                "Авторизация не прошла. Проверьте данные авторизации.");
        if (!response.IsSuccessStatusCode || response.Content == null)
        {
            throw new HttpRequestException(
                $"Ошибка при отправке TSU-сообщения. Статус: {response.StatusCode}. Причина: {response.ReasonPhrase}");
        }

        return await response.Content.ReadFromJsonAsync<TsuResponseMessage>(cancellationToken: cancellationToken)!;
    }

    public async Task<TransportOrderResponseMessage> SendTransportOrderChangeMessage(
        TransportOrderChangeMessage message,
        CancellationToken cancellationToken = default)
    {
        using var client = CreateClient();
        var response = await client.PostAsJsonAsync("api/order/change", message, cancellationToken);
        if (response.StatusCode is HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException(
                "Авторизация не прошла. Проверьте данные авторизации.");
        if (!response.IsSuccessStatusCode || response.Content == null)
        {
            throw new HttpRequestException(
                $"Ошибка при отправке сообщения об изменении заказа. Статус: {response.StatusCode}. Причина: {response.ReasonPhrase}");
        }

        return await response.Content.ReadFromJsonAsync<TransportOrderResponseMessage>(
            cancellationToken: cancellationToken)!;
    }

    public async Task<bool> SendTransportOrderRelaunchMessage(
        TransportOrderRelaunchMessage message,
        CancellationToken cancellationToken = default)
    {
        using var client = CreateClient();
        var response = await client.PostAsJsonAsync("api/order/relaunch", message, cancellationToken);
        return response.IsSuccessStatusCode;
    }
}