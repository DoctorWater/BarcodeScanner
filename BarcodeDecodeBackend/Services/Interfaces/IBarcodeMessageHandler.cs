using BarcodeDecodeLib.Models.Dtos.Messages;

namespace BarcodeDecodeBackend.Services.Interfaces;

public interface IBarcodeMessageHandler
{
    Task<BarcodeResponseMessageBatch> HandleBarcodes(IEnumerable<string> barcodes);
}