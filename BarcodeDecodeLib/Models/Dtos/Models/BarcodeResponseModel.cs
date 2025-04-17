using BarcodeDecodeLib.Entities;
using BarcodeDecodeLib.Models.Dtos.Messages;

namespace BarcodeDecodeLib.Models.Dtos.Models;

public class BarcodeResponseModel : HttpMessage
{
    public List<TransportStorageUnit> TransportStorageUnits { get; init; } = new();
    public List<TransportOrder> TransportOrders { get; init; } = new();
}