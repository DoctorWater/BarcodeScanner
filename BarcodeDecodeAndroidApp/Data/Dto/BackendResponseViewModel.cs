using BarcodeDecodeLib.Models.Dtos.Messages.LocationTicket;

public class BackendResponseViewModel
{
    public List<TransportOrderViewModel> TransportOrders {get; init;} = new();
    public List<TransportStorageUnitViewModel> TransportStorageUnits {get; init;} = new();    
}