using System.Text.Json.Serialization;
using BarcodeDecodeLib.Entities;
using BarcodeDecodeLib.Models.Dtos.Messages.LocationTicket;
using BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;
using BarcodeDecodeLib.Models.Enums;

namespace BarcodeDecodeLib.Models.Dtos.Messages.Tsu;

public class TsuResponseMessage
{
    public TsuResponseMessage(TransportStorageUnit tsu)
    {
        Id = tsu.Id;
        Barcode = tsu.Barcode;
        UpdatedOn = tsu.UpdatedOn;
        CreatedOn = tsu.CreatedOn;
        LocationTickets = tsu.LocationTickets.Select(x => new LocationTicketResponseDto(x)).ToList();
        TransportOrder = tsu.TransportOrder is not null ? new TransportOrderResponseMessage(tsu.TransportOrder) : null;
        Status = tsu.Status;
    }

    //Used in deserialization
    [JsonConstructor]
    public TsuResponseMessage(int id, DateTimeOffset createdOn, DateTimeOffset updatedOn, TsuStatusEnum status, string barcode, TransportOrderResponseMessage? transportOrder)
    {
        Id = id;
        CreatedOn = createdOn;
        UpdatedOn = updatedOn;
        Status = status;
        Barcode = barcode;
        TransportOrder = transportOrder;
    }

    public int Id { get; set; }
    public DateTimeOffset CreatedOn { get; init; }
    public DateTimeOffset UpdatedOn { get; set; }
    public TsuStatusEnum Status { get; set; }
    public string Barcode { get; set; }
    
    public List<LocationTicketResponseDto> LocationTickets { get; init; } = new();
    
    public TransportOrderResponseMessage? TransportOrder { get; set; }
}