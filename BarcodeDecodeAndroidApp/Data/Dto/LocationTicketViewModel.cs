using BarcodeDecodeLib.Models.Dtos.Messages.LocationTicket;

public class LocationTicketViewModel
{
    public int ID {get; set;}
    public int DepartureLocation {get; set;}
    public int PlannedLocation {get; set;}
    public int? ArrivedAtLocation {get; set;}
    public string Status {get; set;}
    public DateTimeOffset? ArrivedOn {get; set;}
    public bool IsSuccess {get; set;}


    public static LocationTicketViewModel GetFrom(LocationTicketResponseDto ticket)
    {
        var plannedLocation = ticket.PlannedLocations.Any() ? ticket.PlannedLocations[0] : 0;
        return new()
        {
            ID = ticket.Id,
            DepartureLocation = ticket.DepartureLocation,
            Status = ticket.Status.ToString(),
            PlannedLocation = plannedLocation,
            ArrivedAtLocation = ticket.ArrivedAtLocation,
            ArrivedOn = ticket.ArrivedOn,
            IsSuccess = plannedLocation.Equals(ticket.ArrivedAtLocation)
        };
    }
}