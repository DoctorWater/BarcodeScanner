using BarcodeDecodeLib.Entities;

namespace BarcodeDecodeLib.Models.Messages;

public class BarcodeResponseMessage
{
    public List<TransportStorageUnit> TransportStorageUnits { get; init; } = new();
    public List<TransportOrder> TransportOrders { get; init; } = new();
}