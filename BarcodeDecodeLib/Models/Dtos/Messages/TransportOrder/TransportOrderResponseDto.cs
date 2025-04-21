using System.Text.Json.Serialization;
using BarcodeDecodeLib.Models.Enums;

namespace BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;

public class TransportOrderResponseDto
{
    public TransportOrderResponseDto(Entities.TransportOrder order)
    {
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
    public TransportOrderResponseDto(int id, string externalId, string barcode, DateTimeOffset createdOn, TransportOrderStatusEnum status, DateTimeOffset? closedOn)
    {
        Id = id;
        ExternalId = externalId;
        Barcode = barcode;
        CreatedOn = createdOn;
        Status = status;
        ClosedOn = closedOn;
    }

    public int Id { get; set; }
    public string ExternalId { get; init; }
    public string Barcode { get; set; }
    
    public List<int> Destinations { get; set; } = new();

    public DateTimeOffset CreatedOn { get; init; }
    public TransportOrderStatusEnum Status { get; set; }
    public DateTimeOffset? ClosedOn { get; set; }
}