using BarcodeDecodeLib.Models.Dtos;
using BarcodeDecodeLib.Models.Messages;
using MassTransit;
using Microsoft.Extensions.Options;

namespace BarcodeDecodeFrontend.Data.Services.Messaging;

public class BarcodeRequestMessageBatchPublisher
{
    private HttpAddresses _addresses;

    public BarcodeRequestMessageBatchPublisher(IOptions<HttpAddresses> addresses)
    {
        _addresses = addresses.Value;
    }

    public async Task SendBarcodeRequest(BarcodeRequestMessageBatch message)
    {
        using var client = new HttpClient
        {
            BaseAddress = new Uri(_addresses.BarcodeDecodeBackendAddress)
        };
        
        await client.PostAsJsonAsync("api/barcode/batch", message);
    }
}