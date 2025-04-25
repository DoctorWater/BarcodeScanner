using BarcodeDecodeLib.Models.Enums;

namespace BarcodeDecodeLib.Models.Dtos.Messages.Tsu;

public class TsuChangeMessage : HttpMessage
{
    public TsuChangeMessage(int id)
    {
        CorrelationId = Guid.NewGuid();
        Id = id;
    }

    public int Id { get; }
    
    public TsuStatusEnum? Status { get; init; }
    public string? Barcode { get; init; }
}