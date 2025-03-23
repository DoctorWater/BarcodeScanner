using BarcodeDecodeLib.Entities;
using BarcodeDecodeLib.Models.Enums;

namespace BarcodeDecodeFrontend.Data.Dtos;

public class OrderDto
{
    public string ExternalId { get; set; }
    public string? Barcode { get; set; }
    public int? Destination { get; set; }
    public TransportOrderStatusEnum Status { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset? ClosedOn { get; set; }
    public bool IsOkay { get; set; }

    public static OrderDto? GetFrom(TransportOrder? order)
    {
        if (order is null)
            return null;
        var destination = order.Destinations is not null && order.Destinations.Any() ? order.Destinations[0] : 0;
        bool isOkay = false;
        if (destination is not 0
            && order.Status != TransportOrderStatusEnum.Error
            && order.Barcode is not null
            && order.Barcode != string.Empty)
        {
            isOkay = true;
        }
        return new()
        {
            ExternalId = order.ExternalId,
            Barcode = order.Barcode,
            Status = order.Status,
            CreatedOn = order.CreatedOn,
            Destination = destination,
            ClosedOn = order.ClosedOn,
            IsOkay = isOkay
        };
    }
}