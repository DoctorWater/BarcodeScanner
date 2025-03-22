namespace BarcodeDecodeFrontend.Data.Models;

public class BarcodeModel
{
    public BarcodeModel(string barcode)
    {
        Barcode = barcode;
    }

    public string Barcode { get; set; }
}