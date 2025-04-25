using System.Text.Json.Serialization;
using BarcodeDecodeLib.Models.Enums;

namespace BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;

public class TransportOrderResponseMessage : HttpMessage
{
    public TransportOrderResponseMessage(Guid? correlationId, Entities.TransportOrder order)
    {
        CorrelationId = correlationId;
        Id = order.Id;
        ExternalId = order.ExternalId;
        Barcode = order.Barcode;
        Destinations = order.Destinations;
        CreatedOn = order.CreatedOn;
        Status = order.Status;
        ClosedOn = order.ClosedOn;
    }

    //Used in deserialization
    [JsonConstructor]
    public TransportOrderResponseMessage(Guid? correlationId, int id, string externalId, string barcode, DateTimeOffset createdOn, TransportOrderStatusEnum status, DateTimeOffset? closedOn)
    {
        CorrelationId = correlationId;
        Id = id;
        ExternalId = externalId;
        Barcode = barcode;
        CreatedOn = createdOn;
        Status = status;
        ClosedOn = closedOn;
    }

    public int Id { get; init; }
    public string ExternalId { get; init; }
    public string Barcode { get; init; }
    
    public List<int> Destinations { get; init; } = new();

    public DateTimeOffset CreatedOn { get; init; }
    public TransportOrderStatusEnum Status { get; init; }
    public DateTimeOffset? ClosedOn { get; init; }
}