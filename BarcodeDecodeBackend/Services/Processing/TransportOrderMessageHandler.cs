using BarcodeDecodeBackend.Services.Interfaces;
using BarcodeDecodeDataAccess.Interfaces;
using BarcodeDecodeLib.Entities;
using BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;

namespace BarcodeDecodeBackend.Services.Processing;

public class TransportOrderMessageHandler : ITransportOrderMessageHandler
{
    private readonly ITransportOrderRepository _transportOrderRepository;

    public TransportOrderMessageHandler(ITransportOrderRepository transportOrderRepository)
    {
        _transportOrderRepository = transportOrderRepository;
    }

    public Task<TransportOrder?> HandleOrderChange(TransportOrderChangeMessage orderChangeMessage)
    {
        var result = _transportOrderRepository.Update(orderChangeMessage);
        return result;
    }

    public Task<bool> HandleOrderRelaunch(TransportOrderRelaunchMessage transportOrderRelaunchMessage)
    {
        return _transportOrderRepository.Relaunch(transportOrderRelaunchMessage);
    }
}