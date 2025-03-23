using BarcodeDecodeLib.Entities;
using BarcodeDecodeLib.Models.Enums;

namespace BarcodeDecodeFrontend.Data.Dtos;

public class TsuDto
{
    public int Id { get; set; }
    public string? Barcode { get; set; }
    public TsuStatusEnum Status { get; set; }
    public List<LocationTicket> LocationTickets { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public OrderDto? InnerOrder { get; set; }
    public bool IsOkay { get; set; }

    public static TsuDto? GetFrom(TransportStorageUnit? unit)
    {
        if (unit is null)
            return null;
        var isOkay = false;
        var isAnyTransportationWasWrong = unit.LocationTickets
            .Any(x => x.Status != TransportLocationTicketStatus.OnTrack && x.ArrivedAtLocation is not null
                                                                        && x.PlannedLocations?.Contains(x.ArrivedAtLocation.Value) is false);

        if (unit.Status != TsuStatusEnum.None && isAnyTransportationWasWrong is false)
        {
            isOkay = true;
        }

        return new()
        {
            Id = unit.Id,
            Barcode = unit.Barcode,
            Status = unit.Status,
            LocationTickets = unit.LocationTickets,
            UpdatedOn = unit.UpdatedOn,
            IsOkay = isOkay,
            InnerOrder = OrderDto.GetFrom(unit.TransportOrder)
        };
    }
}