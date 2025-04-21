using BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;
using BarcodeDecodeLib.Models.Enums;

namespace BarcodeDecodeFrontend.Data.Dtos;

public class OrderEditDto
{
    public int Id { get; set; }
    public string? Barcode { get; set; }
    public TransportOrderStatusEnum Status { get; set; }

    public static OrderEditDto? GetFrom(TransportOrderResponseDto? order)
    {
        if (order is null)
            return null;
        return new()
        {
            Id = order.Id,
            Barcode = order.Barcode,
            Status = order.Status,
        };
    }
}