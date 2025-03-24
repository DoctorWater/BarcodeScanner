namespace BarcodeDecodeFrontend.Data.Models;

public class BarcodeModel
{
    public BarcodeModel(Guid id, string barcode)
    {
        Id = id;
        Barcode = barcode;
    }

    public Guid Id { get; set; }
    public string Barcode { get; set; }
}