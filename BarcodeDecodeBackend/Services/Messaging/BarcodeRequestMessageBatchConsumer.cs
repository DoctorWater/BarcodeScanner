using BarcodeDecodeLib.Models.Messages;
using MassTransit;

namespace BarcodeDecodeBackend.Services.Messaging;

public class BarcodeRequestMessageBatchConsumer : IConsumer<BarcodeRequestMessageBatch>
{
    public Task Consume(ConsumeContext<BarcodeRequestMessageBatch> context)
    {
        
    }
}