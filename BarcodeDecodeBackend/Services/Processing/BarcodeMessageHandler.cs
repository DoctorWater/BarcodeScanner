using System.Diagnostics;
using BarcodeDecodeBackend.Services.Interfaces;
using BarcodeDecodeDataAccess;
using BarcodeDecodeLib.Models.Dtos.Messages;
using BarcodeDecodeLib.Models.Dtos.Models;
using Microsoft.EntityFrameworkCore;

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
        BarcodeResponseMessageBatch message = new BarcodeResponseMessageBatch(result);
        return Task.FromResult(message);
    }

    private BarcodeResponseModel CreateBarcodeResponse(string barcode)
    {
        var tsus = _dbContext.TransportStorageUnits.Where(x => x.Barcode == barcode).Include(x => x.LocationTickets).ToList();
        var orders = _dbContext.TransportOrders.Where(x => x.Barcode == barcode).ToList();
        return new BarcodeResponseModel()
        {
            TransportOrders = orders,
            TransportStorageUnits = tsus
        };
    }
}