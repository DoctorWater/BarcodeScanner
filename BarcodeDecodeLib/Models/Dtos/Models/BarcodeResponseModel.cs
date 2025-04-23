using BarcodeDecodeLib.Models.Dtos.Messages;
using BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;
using BarcodeDecodeLib.Models.Dtos.Messages.Tsu;

namespace BarcodeDecodeLib.Models.Dtos.Models;

public class BarcodeResponseModel
{
    public IEnumerable<TsuResponseMessage> TransportStorageUnits { get; init; } = new List<TsuResponseMessage>();
    public IEnumerable<TransportOrderResponseMessage> TransportOrders { get; init; } = new List<TransportOrderResponseMessage>();
}