using System.ComponentModel.DataAnnotations.Schema;
using BarcodeDecodeLib.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BarcodeDecodeLib.Entities;

[Index(nameof(Barcode), nameof(Status), IsUnique = false)]
[Index(nameof(Status), nameof(UpdatedOn), IsUnique = false)]
[Index(nameof(UpdatedOn), IsUnique = false)]
public class TransportStorageUnit
{
    public int Id { get; set; }
    public DateTimeOffset CreatedOn { get; init; }
    public DateTimeOffset UpdatedOn { get; set; }
    public TsuStatusEnum Status { get; set; }
    public string Barcode { get; set; }
    public List<LocationTicket> LocationTickets { get; set; } = new();
    //public TransportDataHistory TransportDataHistory { get; set; } = new();
    public TransportStorageUnit(string barcode)
    {
        CreatedOn = DateTimeOffset.Now;
        UpdatedOn = DateTimeOffset.Now;
        Status = TsuStatusEnum.Active;
        Barcode = barcode;
    }
    public int? TransportOrderId { get; set; }

    [ForeignKey(nameof(TransportOrderId))]
    public TransportOrder? TransportOrder { get; set; }
}