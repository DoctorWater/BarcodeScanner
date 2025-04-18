using System.Collections.Immutable;
using BarcodeDecodeDataAccess.Interfaces;
using BarcodeDecodeLib.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarcodeDecodeDataAccess.Repositories;

public class TransportStorageUnitRepository : ITransportStorageUnitRepository
{
    private readonly BarcodeDecodeDbContext _dbContext;

    public TransportStorageUnitRepository(BarcodeDecodeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IEnumerable<TransportStorageUnit> GetTsuByBarcode(string barcode)
    {
        return _dbContext.TransportStorageUnits
            .Where(x => x.Barcode == barcode)
            .Include(x => x.LocationTickets)
            .Include(x => x.TransportOrder)
            .ToImmutableList();
    }
}