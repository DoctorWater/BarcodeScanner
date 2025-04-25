using System.Text.Json.Serialization;

namespace BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;

public class TransportOrderRelaunchMessage : HttpMessage
{
    public TransportOrderRelaunchMessage(string barcode)
    {
        CorrelationId = Guid.NewGuid();
        Barcode = barcode;
    }

    [JsonConstructor]
    public TransportOrderRelaunchMessage(Guid correlationId, string barcode, List<int> destinations)
    {
        CorrelationId = correlationId;
        Barcode = barcode;
        Destinations = destinations;
    }

    public string Barcode { get; }
    public List<int> Destinations { get; init; } = new();
}