using BarcodeDecodeDataAccess.Interfaces;
using BarcodeDecodeLib.Entities;
using BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;
using BarcodeDecodeLib.Models.Enums;
using Microsoft.Extensions.Logging;

namespace BarcodeDecodeDataAccess.Repositories;

public class TransportOrderRepository : ITransportOrderRepository
{
    private readonly BarcodeDecodeDbContext _dbContext;
    private readonly ILogger<TransportOrderRepository> _logger;

    public TransportOrderRepository(BarcodeDecodeDbContext dbContext, ILogger<TransportOrderRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public IEnumerable<TransportOrder> GetByBarcode(string barcode)
    {
        var result = _dbContext.TransportOrders.Where(x => x.Barcode == barcode);
        _logger.LogDebug("{foundCount} orders found by barcode {barcode}", result.Count(), barcode);
        return result;
    }

    public async Task<TransportOrder?> Update(TransportOrderChangeMessage message)
    {
        var order = _dbContext.TransportOrders.FirstOrDefault(x => x.Id == message.Id);
        if (order == null)
        {
            _logger.LogWarning("Transport order with id {id} was not found", message.Id);
            return null;
        }

        order.Status = message.Status ?? order.Status;
        order.Barcode = message.Barcode ?? order.Barcode;
        _logger.LogInformation("Updating order with id {id} to status {status} and barcode {barcode}", message.Id, message.Status, message.Barcode);
        await _dbContext.SaveChangesAsync();
        return order;
    }

    public async Task<bool> Relaunch(TransportOrderRelaunchMessage message)
    {
        var newOrder = new TransportOrder(message.Barcode, Guid.NewGuid().ToString(), message.Destinations,
            DateTimeOffset.Now.ToUniversalTime(), TransportOrderStatusEnum.Created);
        _logger.LogDebug("Creating new order {newOrder}", newOrder);
        _dbContext.TransportOrders.Add(newOrder);
        return await _dbContext.SaveChangesAsync() > 0;
    }
}