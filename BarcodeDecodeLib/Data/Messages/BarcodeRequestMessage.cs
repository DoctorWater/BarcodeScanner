namespace BarcodeDecodeLib.Data.Messages;

public class BarcodeRequestMessage : BrokerMessage
{
    public Guid CorrelationId { get; set; }
    public string Text { get; set; }

    public BarcodeRequestMessage(string text)
    {
        Text = text;
    }
}