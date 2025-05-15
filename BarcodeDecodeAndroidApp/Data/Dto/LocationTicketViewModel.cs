using BarcodeDecodeLib.Models.Dtos.Messages.LocationTicket;

public class LocationTicketViewModel
{
    public LocationTicketViewModel()
    {
    }

    public LocationTicketViewModel(LocationTicketResponseDto locationTicketResponse)
    {
        ID = locationTicketResponse.Id;
        DepartureLocation = locationTicketResponse.DepartureLocation;
        PlannedLocation = locationTicketResponse.PlannedLocations.Any() ? locationTicketResponse.PlannedLocations[0] : 0;
        ArrivedAtLocation = locationTicketResponse.ArrivedAtLocation;
        Status = locationTicketResponse.Status.ToString();
        ArrivedOn = locationTicketResponse.ArrivedOn;
        IsSuccess = PlannedLocation.Equals(ArrivedAtLocation);
    }

    

    public int ID {get; set;}
    public int DepartureLocation {get; set;}
    public int PlannedLocation {get; set;}
    public int? ArrivedAtLocation {get; set;}
    public string Status {get; set;}
    public DateTimeOffset? ArrivedOn {get; set;}
    public bool IsSuccess {get; set;}
}