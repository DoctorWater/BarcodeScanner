using BarcodeDecodeLib.Entities;
using BarcodeDecodeLib.Models.Dtos.Messages.Tsu;

namespace BarcodeDecodeDataAccess.Interfaces;

public interface ITransportStorageUnitRepository
{
    IEnumerable<TransportStorageUnit> GetByBarcode(string barcode);
    Task<TransportStorageUnit?> Update(TsuChangeMessage message);
}