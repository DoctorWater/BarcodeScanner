using BarcodeDecodeDataAccess.Interfaces;
using BarcodeDecodeLib.Entities;
using BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;

namespace BarcodeDecodeDataAccess.Repositories;

public class TransportOrderRepository : ITransportOrderRepository
{
    private readonly BarcodeDecodeDbContext _dbContext;

    public TransportOrderRepository(BarcodeDecodeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IEnumerable<TransportOrder> GetByBarcode(string barcode)
    {
        return _dbContext.TransportOrders.Where(x => x.Barcode == barcode);
    }

    public async Task<TransportOrder?> Update(TransportOrderChangeMessage message)
    {
        var order = _dbContext.TransportOrders.FirstOrDefault(x => x.Id == message.Id);
        if (order == null) return null;
        order.Status = message.Status ?? order.Status;
        order.Barcode = message.Barcode ?? order.Barcode;
        await _dbContext.SaveChangesAsync();
        return order;
    }
}