using System.Text.Json.Serialization;
using BarcodeDecodeLib.Models.Enums;

namespace BarcodeDecodeLib.Models.Dtos.Messages.LocationTicket;

public class LocationTicketResponseDto
{
    public LocationTicketResponseDto(Entities.LocationTicket locationTicket)
    {
        Id = locationTicket.Id;
        CreatedOn = locationTicket.CreatedOn;
        DepartureLocation = locationTicket.DepartureLocation;
        Status = locationTicket.Status;
        PlannedLocations = locationTicket.PlannedLocations;
        ArrivedAtLocation = locationTicket.ArrivedAtLocation;
        ArrivedOn = locationTicket.ArrivedOn;
        SortingErrorCode = locationTicket.SortingErrorCode;
        ErrorMessage = locationTicket.ErrorMessage;
    }

    //Used in deserialization
    [JsonConstructor]
    public LocationTicketResponseDto(int id, DateTimeOffset createdOn, int departureLocation, TransportLocationTicketStatus status, int? arrivedAtLocation, DateTimeOffset? arrivedOn, int sortingErrorCode, string? errorMessage)
    {
        Id = id;
        CreatedOn = createdOn;
        DepartureLocation = departureLocation;
        Status = status;
        ArrivedAtLocation = arrivedAtLocation;
        ArrivedOn = arrivedOn;
        SortingErrorCode = sortingErrorCode;
        ErrorMessage = errorMessage;
    }

    public int Id { get; init; }
    public DateTimeOffset CreatedOn { get; init; }
    
    public int DepartureLocation { get; init; }
    public TransportLocationTicketStatus Status { get; init; }
    
    public List<int> PlannedLocations { get; init; } = new();

    public int? ArrivedAtLocation { get; init; }
    public DateTimeOffset? ArrivedOn { get; init; }

    public int SortingErrorCode { get; init; }
    public string? ErrorMessage { get; init; }
}