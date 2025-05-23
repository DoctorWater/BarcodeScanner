using System.Text.Json.Serialization;

namespace BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;

public class TransportOrderRelaunchMessage : HttpMessage
{
    
    public TransportOrderRelaunchMessage(string barcode)
    {
        CorrelationId = Guid.NewGuid();
        Barcode = barcode;
    }

    public string Barcode { get; }
    public List<int> Destinations { get; init; } = new();
}