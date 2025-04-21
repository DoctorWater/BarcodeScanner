using BarcodeDecodeBackend.Services.Interfaces;
using BarcodeDecodeDataAccess.Interfaces;
using BarcodeDecodeLib.Entities;
using BarcodeDecodeLib.Models.Dtos.Messages.Tsu;

namespace BarcodeDecodeBackend.Services.Processing;

public class TsuMessageHandler : ITsuMessageHandler
{
    private ITransportStorageUnitRepository _transportStorageUnitRepository;

    public TsuMessageHandler(ITransportStorageUnitRepository transportStorageUnitRepository)
    {
        _transportStorageUnitRepository = transportStorageUnitRepository;
    }

    public Task<TransportStorageUnit?> HandleTsuChange(TsuChangeMessage tsuChangeMessage)
    {
        var result = _transportStorageUnitRepository.Update(tsuChangeMessage);
        return result;
    }
}