using BarcodeDecodeBackend.Services.Interfaces;
using BarcodeDecodeDataAccess.Interfaces;
using BarcodeDecodeLib.Models.Dtos.Messages.Barcode;
using BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;
using BarcodeDecodeLib.Models.Dtos.Messages.Tsu;
using BarcodeDecodeLib.Models.Dtos.Models;

namespace BarcodeDecodeBackend.Services.Processing;

public class BarcodeMessageHandler : IBarcodeMessageHandler
{
    private readonly ITransportStorageUnitRepository _transportStorageUnitRepository;
    private readonly ITransportOrderRepository _transportOrderRepository;

    public BarcodeMessageHandler(ITransportStorageUnitRepository transportStorageUnitRepository, ITransportOrderRepository transportOrderRepository)
    {
        _transportStorageUnitRepository = transportStorageUnitRepository;
        _transportOrderRepository = transportOrderRepository;
    }

    public Task<BarcodeResponseMessageBatch> HandleBarcodes(IEnumerable<string> barcodes)
    {
        var result = barcodes.Select(CreateBarcodeResponse).ToList();
        BarcodeResponseMessageBatch message = new BarcodeResponseMessageBatch(result);
        return Task.FromResult(message);
    }

    private BarcodeResponseModel CreateBarcodeResponse(string barcode)
    {
        var transportStorageUnits = _transportStorageUnitRepository.GetByBarcode(barcode).Select(x => new TsuResponseMessage(x));
        var orders = _transportOrderRepository.GetByBarcode(barcode).Select(x => new TransportOrderResponseMessage(x));
        return new BarcodeResponseModel()
        {
            TransportOrders = orders,
            TransportStorageUnits = transportStorageUnits
        };
    }
}