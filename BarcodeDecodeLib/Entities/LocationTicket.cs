using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BarcodeDecodeLib.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BarcodeDecodeLib.Entities;
[Index(nameof(DepartureLocation), IsUnique = false)]
[Index(nameof(ArrivedAtLocation), IsUnique = false)]
[Index(nameof(CreatedOn), IsUnique = false)]
[Index(nameof(TransportStorageUnitId), IsUnique = false)]
public class LocationTicket
{
    public int Id { get; set; }
    public DateTimeOffset CreatedOn { get; init; }
    public int DepartureLocation { get; init; }
    public TransportLocationTicketStatus Status { get; set; }
    public virtual List<int> PlannedLocations { get; set; }
    public int? ArrivedAtLocation { get; set; }
    public DateTimeOffset? ArrivedOn { get; set; }
    public int SortingErrorCode { get; set; } // LlcSortReportError
    [MaxLength(30, ErrorMessage = "Error message can not be longer than 30")]
    public string? ErrorMessage { get; set; }
    public int TransportStorageUnitId { get; set; }
}