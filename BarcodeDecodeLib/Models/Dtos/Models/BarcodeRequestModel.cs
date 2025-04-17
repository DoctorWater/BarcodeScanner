namespace BarcodeDecodeLib.Models.Dtos.Models;

public class BarcodeRequestModel
{
    public string BarcodeText { get; set; }

    public BarcodeRequestModel(string barcodeText)
    {
        BarcodeText = barcodeText;
    }
}