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

    public Task<TransportOrder?> HandleOrderChange(TransportOrderChangeMessage tsuChangeMessage)
    {
        var result = _transportOrderRepository.Update(tsuChangeMessage);
        return result;
    }
}