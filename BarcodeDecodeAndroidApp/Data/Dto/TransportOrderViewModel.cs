using BarcodeDecodeLib.Models.Dtos.Messages.LocationTicket;
using BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;
using BarcodeDecodeLib.Models.Enums;

public class TransportOrderViewModel
{
    public string ExternalId { get; set; }
    public string? Barcode { get; set; }
    public int? Destination { get; set; }
    public TransportOrderStatusEnum Status { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset? ClosedOn { get; set; }
    public bool IsOkay { get; set; }
    public TransportOrderViewModel(){}

    public TransportOrderViewModel(TransportOrderResponseMessage order)
    {

        var destination = order.Destinations is not null && order.Destinations.Any() ? order.Destinations[0] : 0;
        bool isOkay = false;
        if (destination is not 0
        && order.Status != TransportOrderStatusEnum.Error
        && string.IsNullOrEmpty(order.Barcode))
        {
            isOkay = true;
        }

        ExternalId = order.ExternalId;
        Barcode = order.Barcode;
        Status = order.Status;
        CreatedOn = order.CreatedOn;
        Destination = destination;
        ClosedOn = order.ClosedOn;
        IsOkay = isOkay;
    }
}