using System.Text.Json.Serialization;

namespace BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;

public class TransportOrderRelaunchMessage
{
    public TransportOrderRelaunchMessage(string barcode)
    {
        Barcode = barcode;
    }

    [JsonConstructor]
    public TransportOrderRelaunchMessage(string barcode, List<int> destinations)
    {
        Barcode = barcode;
        Destinations = destinations;
    }

    public string Barcode { get; }
    public List<int> Destinations { get; init; } = new();
}