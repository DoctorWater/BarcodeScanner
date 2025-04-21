using System.Collections.Immutable;
using BarcodeDecodeDataAccess.Interfaces;
using BarcodeDecodeLib.Entities;
using BarcodeDecodeLib.Models.Dtos.Messages.Tsu;
using Microsoft.EntityFrameworkCore;

namespace BarcodeDecodeDataAccess.Repositories;

public class TransportStorageUnitRepository : ITransportStorageUnitRepository
{
    private readonly BarcodeDecodeDbContext _dbContext;

    public TransportStorageUnitRepository(BarcodeDecodeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IEnumerable<TransportStorageUnit> GetByBarcode(string barcode)
    {
        return _dbContext.TransportStorageUnits
            .Where(x => x.Barcode == barcode)
            .Include(x => x.LocationTickets)
            .Include(x => x.TransportOrder)
            .ToImmutableList();
    }
    
    public async Task<TransportStorageUnit?> Update(TsuChangeMessage message)
    {
        var tsu = _dbContext.TransportStorageUnits
            .Include(x => x.LocationTickets)
            .Include(x => x.TransportOrder)
            .FirstOrDefault(x => x.Id == message.Id);
        if (tsu == null) return null;
        
        tsu.Status = message.Status ?? tsu.Status;
        tsu.Barcode = message.Barcode ?? tsu.Barcode;

        tsu.UpdatedOn = DateTimeOffset.UtcNow;
        await _dbContext.SaveChangesAsync();
        return tsu;
    }
}