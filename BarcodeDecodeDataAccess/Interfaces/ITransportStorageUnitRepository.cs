using BarcodeDecodeLib.Entities;

namespace BarcodeDecodeDataAccess.Interfaces;

public interface ITransportStorageUnitRepository
{
    IEnumerable<TransportStorageUnit> GetTsuByBarcode(string barcode); 
}