namespace BarcodeDecodeFrontend.Data.Dtos;

public class OrderEditDto
{
    public string? Barcode { get; set; }
    public string? PreferableClearing { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }

    public string? Status { get; set; }
}