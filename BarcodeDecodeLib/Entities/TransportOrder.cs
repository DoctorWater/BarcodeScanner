using System.ComponentModel.DataAnnotations;
using BarcodeDecodeLib.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BarcodeDecodeLib.Entities;

[Index(nameof(Barcode), IsUnique = false)]
[Index(nameof(CreatedOn), IsUnique = false)]
[Index(nameof(ClosedOn), IsUnique = false)]
[Index(nameof(Status), IsUnique = false)]
public class TransportOrder
{
    public int Id { get; set; }
    public string ExternalId { get; init; }
    public string Barcode { get; set; }
    public List<int> Destinations { get; set; }
    public DateTimeOffset CreatedOn { get; init; }
    [Required]
    public TransportOrderStatusEnum Status { get; set; }
    public DateTimeOffset? ClosedOn { get; set; }

    public TransportOrder(string barcode, string externalId, List<int> destinations, DateTimeOffset createdOn, TransportOrderStatusEnum status)
    {
        Barcode = barcode;
        ExternalId = externalId;
        Destinations = destinations;
        CreatedOn = createdOn;
        Status = status;
    }
}