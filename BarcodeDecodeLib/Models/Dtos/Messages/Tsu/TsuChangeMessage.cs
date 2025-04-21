using BarcodeDecodeLib.Models.Enums;

namespace BarcodeDecodeLib.Models.Dtos.Messages.Tsu;

public class TsuChangeMessage
{
    public TsuChangeMessage(int id)
    {
        Id = id;
    }

    public int Id { get; }
    
    public TsuStatusEnum? Status { get; init; }
    public string? Barcode { get; init; }
}