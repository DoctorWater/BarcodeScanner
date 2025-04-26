public class ClientPresentationDto
{
    public TransportOrderViewModel Order {get; set;}
    public IEnumerable<LocationTicketViewModel> LocationTickets {get; set;}
}