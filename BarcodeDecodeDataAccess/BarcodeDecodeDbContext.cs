using BarcodeDecodeLib.Entities;
using BarcodeDecodeLib.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BarcodeDecodeDataAccess;

public class BarcodeDecodeDbContext : DbContext
{
    public DbSet<TransportStorageUnit> TransportStorageUnits { get; set; }
    public DbSet<TransportOrder> TransportOrders { get; set; }
    public DbSet<LocationTicket> LocationTickets { get; set; }
        
    public BarcodeDecodeDbContext() { }
    public BarcodeDecodeDbContext(DbContextOptions<BarcodeDecodeDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        SeedData(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
    private void SeedData(ModelBuilder modelBuilder)
    {
        var order1 = new TransportOrder("SomeBarcode1", Guid.NewGuid().ToString(), new List<int>(){13},  DateTime.Now - TimeSpan.FromMinutes(1), TransportOrderStatusEnum.Active)
        {
            Id = 1
        };
        var tsu1 = new TransportStorageUnit("SomeBarcode1")
        {
            Id = 1,
            Status = TsuStatusEnum.Active,
            CreatedOn = DateTime.Now - TimeSpan.FromMinutes(1),
            TransportOrderId = order1.Id,
            UpdatedOn = DateTime.Now - TimeSpan.FromMinutes(1),
        };
        
        var ticket1_1 = new LocationTicket()
        {
            Id = 1,
            Status = TransportLocationTicketStatus.ArrivedAtPlannedLocation,
            ArrivedOn = DateTime.Now - TimeSpan.FromSeconds(30),
            CreatedOn = DateTime.Now - TimeSpan.FromSeconds(40),
            DepartureLocation = 10,
            PlannedLocations = new List<int>(){11, 12},
            ArrivedAtLocation = 11,
            TransportStorageUnitId = tsu1.Id,
        };
        var ticket1_2 = new LocationTicket()
        {
            Id = 2,
            Status = TransportLocationTicketStatus.OnTrack,
            ArrivedOn = DateTime.Now - TimeSpan.FromSeconds(10),
            CreatedOn = DateTime.Now - TimeSpan.FromSeconds(20),
            DepartureLocation = 11,
            PlannedLocations = new List<int>(){13},
            TransportStorageUnitId = tsu1.Id,
        };

        modelBuilder.Entity<LocationTicket>().HasData(ticket1_1);
        modelBuilder.Entity<LocationTicket>().HasData(ticket1_2);
        modelBuilder.Entity<TransportOrder>().HasData(order1);
        modelBuilder.Entity<TransportStorageUnit>().HasData(tsu1);
    }


}