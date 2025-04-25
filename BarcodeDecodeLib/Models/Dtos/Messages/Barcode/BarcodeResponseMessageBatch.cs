using BarcodeDecodeLib.Models.Dtos.Models;

namespace BarcodeDecodeLib.Models.Dtos.Messages.Barcode;

public class BarcodeResponseMessageBatch
{
    public List<BarcodeResponseModel> Messages { get; }
    public BarcodeResponseMessageBatch(List<BarcodeResponseModel> messages)
    {
        Messages = messages;
    }
}