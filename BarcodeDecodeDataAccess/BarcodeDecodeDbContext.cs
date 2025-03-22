using BarcodeDecodeLib.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarcodeDecodeDataAccess;

public class BarcodeDecodeDbContext : DbContext
{
    public DbSet<TransportStorageUnit> TransportStorageUnits { get; set; }
    public DbSet<TransportOrder> TransportOrders { get; set; }
    public DbSet<LocationTicket> LocationTickets { get; set; }
        
    public BarcodeDecodeDbContext() { }
    public BarcodeDecodeDbContext(DbContextOptions<BarcodeDecodeDbContext> options) : base(options) { }
}