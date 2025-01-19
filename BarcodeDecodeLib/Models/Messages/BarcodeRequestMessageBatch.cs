namespace BarcodeDecodeLib.Models.Messages;

public class BarcodeRequestMessageBatch
{
    public List<BarcodeRequestMessage> Messages { get; set; } = new();

    public BarcodeRequestMessageBatch(List<BarcodeRequestMessage> messages)
    {
        Messages = messages;
    }
}