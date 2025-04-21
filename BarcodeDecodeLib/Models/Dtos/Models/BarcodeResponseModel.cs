using BarcodeDecodeLib.Models.Dtos.Messages;
using BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;
using BarcodeDecodeLib.Models.Dtos.Messages.Tsu;

namespace BarcodeDecodeLib.Models.Dtos.Models;

public class BarcodeResponseModel : HttpMessage
{
    public IEnumerable<TsuResponseDto> TransportStorageUnits { get; init; } = new List<TsuResponseDto>();
    public IEnumerable<TransportOrderResponseDto> TransportOrders { get; init; } = new List<TransportOrderResponseDto>();
}