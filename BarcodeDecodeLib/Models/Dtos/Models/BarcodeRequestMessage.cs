namespace BarcodeDecodeLib.Models.Dtos.Models;

public class BarcodeRequestMessage
{
    public string BarcodeText { get; init; }

    public BarcodeRequestMessage(string barcodeText)
    {
        BarcodeText = barcodeText;
    }
}