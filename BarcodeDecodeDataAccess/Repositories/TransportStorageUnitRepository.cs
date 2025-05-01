using System.Collections.Immutable;
using BarcodeDecodeDataAccess.Interfaces;
using BarcodeDecodeLib.Entities;
using BarcodeDecodeLib.Models.Dtos.Messages.Tsu;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BarcodeDecodeDataAccess.Repositories;

public class TransportStorageUnitRepository : ITransportStorageUnitRepository
{
    private readonly BarcodeDecodeDbContext _dbContext;
    private readonly ILogger<TransportStorageUnitRepository> _logger;

    public TransportStorageUnitRepository(BarcodeDecodeDbContext dbContext, ILogger<TransportStorageUnitRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public IEnumerable<TransportStorageUnit> GetByBarcode(string barcode)
    {
        var result = _dbContext.TransportStorageUnits
            .Where(x => x.Barcode == barcode)
            .Include(x => x.LocationTickets)
            .Include(x => x.TransportOrder)
            .ToImmutableList();
        _logger.LogDebug("Found {Count} transport storage units with barcode {Barcode}", result.Count, barcode);
        return result;
    }
    
    public async Task<TransportStorageUnit?> Update(TsuChangeMessage message)
    {
        var tsu = _dbContext.TransportStorageUnits
            .Include(x => x.LocationTickets)
            .Include(x => x.TransportOrder)
            .FirstOrDefault(x => x.Id == message.Id);
        if (tsu == null)
        {
            _logger.LogWarning("Transport storage unit with id {Id} not found", message.Id);
            return null;
        }

        tsu.Status = message.Status ?? tsu.Status;
        tsu.Barcode = message.Barcode ?? tsu.Barcode;

        tsu.UpdatedOn = DateTimeOffset.UtcNow;

        _logger.LogInformation("Updating transport storage unit with id {Id} to status {Status} and barcode {Barcode}", message.Id, message.Status, message.Barcode);

        await _dbContext.SaveChangesAsync();
        return tsu;
    }
}