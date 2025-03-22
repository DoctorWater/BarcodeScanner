using BarcodeDecodeLib.Models.Messages;
using MassTransit;

namespace BarcodeDecodeFrontend.Data.Services.Messaging;

public class BarcodeRequestMessageBatchPublisher
{
    private IBus _bus;

    public BarcodeRequestMessageBatchPublisher(IBus bus)
    {
        _bus = bus;
    }

    public async Task SendBarcodeRequest(BarcodeRequestMessageBatch message)
    {
        await _bus.Publish(message);
    }
}