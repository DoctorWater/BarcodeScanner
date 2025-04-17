using BarcodeDecodeLib.Models.Dtos.Models;

namespace BarcodeDecodeLib.Models.Dtos.Messages;

public class BarcodeRequestMessageBatch : HttpMessage
{
    public List<BarcodeRequestModel> Messages { get; }

    public BarcodeRequestMessageBatch(List<BarcodeRequestModel> messages)
    {
        CorrelationId = Guid.NewGuid();
        Messages = messages;
    }
}