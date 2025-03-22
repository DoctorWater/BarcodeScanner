using System.Diagnostics;
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

    public Task<BarcodeResponseMessageBatch> HandleBarcodes(IEnumerable<string> barcodes)
    {
        var result = barcodes.Select(CreateBarcodeResponse).ToList();
        BarcodeResponseMessageBatch message = new BarcodeResponseMessageBatch()
        {
            Messages = result
        };
        return Task.FromResult(message);
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