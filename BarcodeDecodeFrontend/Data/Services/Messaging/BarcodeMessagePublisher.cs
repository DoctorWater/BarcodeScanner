using BarcodeDecodeLib.Entities;
using BarcodeDecodeLib.Models.Dtos;
using BarcodeDecodeLib.Models.Dtos.Configs;
using BarcodeDecodeLib.Models.Dtos.Messages;
using MassTransit;
using Microsoft.Extensions.Options;

namespace BarcodeDecodeFrontend.Data.Services.Messaging;

public class BarcodeMessagePublisher
{
    private HttpAddresses _addresses;

    public BarcodeMessagePublisher(IOptions<HttpAddresses> addresses)
    {
        _addresses = addresses.Value;
    }

    public async Task<BarcodeResponseMessageBatch> SendBarcodeRequest(BarcodeRequestMessageBatch message)
    {
        using var client = new HttpClient
        {
            BaseAddress = new Uri(_addresses.BarcodeDecodeBackendAddress)
        };

        HttpResponseMessage response = await client.PostAsJsonAsync("api/barcode/batch", message);
        if (response?.Content is null || response.IsSuccessStatusCode is false)
        {
            throw new HttpRequestException(
                $"Ошибка при отправке запроса. Статус: {response?.StatusCode.ToString() ?? "нет ответа"}. Причина: {response?.ReasonPhrase}");
        }

        BarcodeResponseMessageBatch responseBatch =
            await response.Content.ReadFromJsonAsync<BarcodeResponseMessageBatch>();
        return responseBatch;
    }

    public async Task<bool> SendTsuChangeMessage(TransportStorageUnit tsu)
    {
        using var client = new HttpClient
        {
            BaseAddress = new Uri(_addresses.BarcodeDecodeBackendAddress)
        };

        HttpResponseMessage response = await client.PostAsJsonAsync("api/tsu/change", tsu);
        return response?.Content is not null && response.IsSuccessStatusCode;
    }
}