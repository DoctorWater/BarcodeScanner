using BarcodeDecodeDataAccess.Interfaces;
using BarcodeDecodeLib.Entities;

namespace BarcodeDecodeDataAccess.Repositories;

public class TransportOrderRepository : ITrasportOrderRepository
{
    private readonly BarcodeDecodeDbContext _dbContext;

    public TransportOrderRepository(BarcodeDecodeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IEnumerable<TransportOrder> GetTransportOrdersByBarcode(string barcode)
    {
        return _dbContext.TransportOrders.Where(x => x.Barcode == barcode);
    }
}