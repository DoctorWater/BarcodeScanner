using BarcodeDecodeLib.Entities;
using BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;
using BarcodeDecodeLib.Models.Dtos.Messages.Tsu;

namespace BarcodeDecodeBackend.Services.Interfaces;

public interface ITransportOrderMessageHandler
{
    Task<TransportOrder?> HandleOrderChange(TransportOrderChangeMessage tsuChangeMessage);
    Task<bool> HandleOrderRelaunch(TransportOrderRelaunchMessage tsuChangeMessage);
}