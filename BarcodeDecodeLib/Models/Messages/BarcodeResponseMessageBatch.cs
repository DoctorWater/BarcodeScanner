namespace BarcodeDecodeLib.Models.Messages;

public class BarcodeResponseMessageBatch
{
    public List<BarcodeResponseMessage> Messages { get; init; } = new();
}