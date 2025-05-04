using BarcodeDecodeLib.Models.Dtos.Messages.Auth;
using BarcodeDecodeLib.Models.Dtos.Messages.Barcode;
using BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;
using BarcodeDecodeLib.Models.Dtos.Messages.Tsu;

namespace BarcodeDecodeFrontend.Data.Services.Messaging;

public interface IHttpMessagePublisher
{
    Task<BarcodeResponseMessageBatch> SendBarcodeRequest(
        BarcodeRequestMessageBatch message,
        CancellationToken cancellationToken = default);

    Task<TsuResponseMessage> SendTsuChangeMessage(
        TsuChangeMessage message,
        CancellationToken cancellationToken = default);

    Task<TransportOrderResponseMessage> SendTransportOrderChangeMessage(
        TransportOrderChangeMessage message,
        CancellationToken cancellationToken = default);

    Task<bool> SendTransportOrderRelaunchMessage(
        TransportOrderRelaunchMessage message,
        CancellationToken cancellationToken = default);
    
    Task<LoginResult?> SendLoginMessage(LoginDto message, CancellationToken cancellationToken = default);
}