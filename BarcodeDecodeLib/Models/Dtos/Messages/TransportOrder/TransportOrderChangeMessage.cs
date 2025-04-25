using BarcodeDecodeLib.Models.Enums;

namespace BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;

public class TransportOrderChangeMessage : HttpMessage
{
    public TransportOrderChangeMessage(int id)
    {
        CorrelationId = Guid.NewGuid();
        Id = id;
    }

    public int Id { get; }
    
    public TransportOrderStatusEnum? Status { get; init; }
    public string? Barcode { get; init; }
}