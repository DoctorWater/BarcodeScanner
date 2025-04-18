using BarcodeDecodeLib.Entities;
using BarcodeDecodeLib.Models.Dtos.Messages;

namespace BarcodeDecodeLib.Models.Dtos.Models;

public class BarcodeResponseModel : HttpMessage
{
    public IEnumerable<TransportStorageUnit> TransportStorageUnits { get; init; } = new List<TransportStorageUnit>();
    public IEnumerable<TransportOrder> TransportOrders { get; init; } = new List<TransportOrder>();
}