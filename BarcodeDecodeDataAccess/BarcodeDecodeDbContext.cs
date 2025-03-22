using BarcodeDecodeLib.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarcodeDecodeDataAccess;

public class BarcodeDecodeDbContext : DbContext
{
    DbSet<TransportStorageUnit> TransportStorageUnits { get; set; }
    DbSet<TransportOrder> TransportOrders { get; set; }
    DbSet<LocationTicket> LocationTickets { get; set; }
        
    public BarcodeDecodeDbContext() { }
    public BarcodeDecodeDbContext(DbContextOptions<BarcodeDecodeDbContext> options) : base(options) { }
}