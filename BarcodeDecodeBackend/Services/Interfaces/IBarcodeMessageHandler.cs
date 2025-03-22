using BarcodeDecodeLib.Models.Messages;

namespace BarcodeDecodeBackend.Services.Interfaces;

public interface IBarcodeMessageHandler
{
    Task HandleBarcodes(IEnumerable<string> barcodes);
}