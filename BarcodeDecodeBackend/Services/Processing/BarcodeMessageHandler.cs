using BarcodeDecodeBackend.Services.Interfaces;
using BarcodeDecodeDataAccess;
using BarcodeDecodeLib.Models.Messages;

namespace BarcodeDecodeBackend.Services.Processing;

public class BarcodeMessageHandler : IBarcodeMessageHandler
{
    private BarcodeDecodeDbContext _dbContext;

    public BarcodeMessageHandler(BarcodeDecodeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task HandleBarcodes(IEnumerable<string> barcodes)
    {
        var result = barcodes.Select(CreateBarcodeResponse).ToList();
        var message = new BarcodeResponseMessageBatch()
        {
            Messages = result
        };
        return Task.CompletedTask;
    }

    private BarcodeResponseMessage CreateBarcodeResponse(string barcode)
    {
        var tsus = _dbContext.TransportStorageUnits.Where(x => x.Barcode == barcode).ToList();
        var orders = _dbContext.TransportOrders.Where(x => x.Barcode == barcode).ToList();
        return new BarcodeResponseMessage()
        {
            TransportOrders = orders,
            TransportStorageUnits = tsus
        };
    }
}