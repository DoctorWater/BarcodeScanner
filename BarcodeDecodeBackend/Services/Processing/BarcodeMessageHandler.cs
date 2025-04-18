using System.Collections.Immutable;
using System.Diagnostics;
using BarcodeDecodeBackend.Services.Interfaces;
using BarcodeDecodeDataAccess;
using BarcodeDecodeDataAccess.Interfaces;
using BarcodeDecodeLib.Models.Dtos.Messages;
using BarcodeDecodeLib.Models.Dtos.Models;
using Microsoft.EntityFrameworkCore;

namespace BarcodeDecodeBackend.Services.Processing;

public class BarcodeMessageHandler : IBarcodeMessageHandler
{
    private readonly ITransportStorageUnitRepository _transportStorageUnitRepository;
    private readonly ITrasportOrderRepository _transportOrderRepository;

    public BarcodeMessageHandler(ITransportStorageUnitRepository transportStorageUnitRepository, ITrasportOrderRepository trasportOrderRepository)
    {
        _transportStorageUnitRepository = transportStorageUnitRepository;
        _transportOrderRepository = trasportOrderRepository;
    }

    public Task<BarcodeResponseMessageBatch> HandleBarcodes(IEnumerable<string> barcodes)
    {
        var result = barcodes.Select(CreateBarcodeResponse).ToList();
        BarcodeResponseMessageBatch message = new BarcodeResponseMessageBatch(result);
        return Task.FromResult(message);
    }

    private BarcodeResponseModel CreateBarcodeResponse(string barcode)
    {
        var transportStorageUnits = _transportStorageUnitRepository.GetTsuByBarcode(barcode);
        var orders = _transportOrderRepository.GetTransportOrdersByBarcode(barcode);
        return new BarcodeResponseModel()
        {
            TransportOrders = orders,
            TransportStorageUnits = transportStorageUnits
        };
    }
}