using BarcodeDecodeLib.Models.Dtos.Models;

namespace BarcodeDecodeLib.Models.Dtos.Messages;

public class BarcodeResponseMessageBatch : HttpMessage
{
    public List<BarcodeResponseModel> Messages { get; }
    public BarcodeResponseMessageBatch(List<BarcodeResponseModel> messages)
    {
        Messages = messages;
    }
}