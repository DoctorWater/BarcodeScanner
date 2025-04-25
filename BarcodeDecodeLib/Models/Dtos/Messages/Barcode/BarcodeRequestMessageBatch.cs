using BarcodeDecodeLib.Models.Dtos.Models;

namespace BarcodeDecodeLib.Models.Dtos.Messages.Barcode;

public class BarcodeRequestMessageBatch : HttpMessage
{
    public List<BarcodeRequestMessage> Messages { get; }

    public BarcodeRequestMessageBatch(List<BarcodeRequestMessage> messages)
    {
        CorrelationId = Guid.NewGuid();
        Messages = messages;
    }
}