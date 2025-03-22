namespace BarcodeDecodeLib.Models.Enums;

public enum TransportLocationTicketStatus
{
    None,
    Created,
    OnTrack,
    ArrivedAtPlannedLocation,
    ArrivedAtWrongLocation,
    ArrivedFromUnknownLocation,
    Lost
}