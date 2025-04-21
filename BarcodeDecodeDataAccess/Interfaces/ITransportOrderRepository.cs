using BarcodeDecodeLib.Entities;
using BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;

namespace BarcodeDecodeDataAccess.Interfaces;

public interface ITransportOrderRepository
{
    IEnumerable<TransportOrder> GetByBarcode(string barcode);
    Task<TransportOrder?> Update(TransportOrderChangeMessage message);
}