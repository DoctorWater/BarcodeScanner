using BarcodeDecodeLib.Entities;
using BarcodeDecodeLib.Models.Dtos.Messages.LocationTicket;
using BarcodeDecodeLib.Models.Dtos.Messages.Tsu;
using BarcodeDecodeLib.Models.Enums;

namespace BarcodeDecodeFrontend.Data.Dtos;

public class TsuEditDto
{
    public int Id { get; set; }
    public string? Barcode { get; set; }
    public TsuStatusEnum Status { get; set; }
    public List<LocationTicketResponseDto> LocationTickets { get; set; }

    public static TsuEditDto? GetFrom(TsuResponseDto unit)
    {
        if (unit is null)
            return null;

        return new()
        {
            Id = unit.Id,
            Barcode = unit.Barcode,
            Status = unit.Status,
            LocationTickets = unit.LocationTickets,
        };
    }
}