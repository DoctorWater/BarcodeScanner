using BarcodeDecodeLib.Entities;
using BarcodeDecodeLib.Models.Dtos.Messages.Tsu;

namespace BarcodeDecodeBackend.Services.Interfaces;

public interface ITsuMessageHandler
{
    Task<TransportStorageUnit?> HandleTsuChange(TsuChangeMessage tsuChangeMessage);
}