using BarcodeDecodeLib.Entities;

namespace BarcodeDecodeDataAccess.Interfaces;

public interface ITrasportOrderRepository
{
    IEnumerable<TransportOrder> GetTransportOrdersByBarcode(string barcode);
}