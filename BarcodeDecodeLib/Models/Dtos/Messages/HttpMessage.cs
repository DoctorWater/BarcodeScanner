namespace BarcodeDecodeLib.Models.Dtos.Messages;

public abstract class HttpMessage
{
    public Guid? CorrelationId { get; set; }
}