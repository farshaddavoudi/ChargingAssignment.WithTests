using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CharginAssignment.WithTests.Infrastructure.Persistence.EFCore.Repositories;

public class ChargeStationRepository(AppDbContext dbContext) : IChargeStationRepository
{
    private readonly DbSet<ChargeStationEntity> _dbSet = dbContext.Set<ChargeStationEntity>();

    public async Task AddChargeStation(ChargeStationEntity chargeStation, CancellationToken cancellationToken)
    {
        await _dbSet.AddAsync(chargeStation, cancellationToken);
    }

    public void UpdateChargeStation(ChargeStationEntity chargeStation)
    {
        _dbSet.Update(chargeStation);
    }

    public void DeleteChargeStation(ChargeStationEntity chargeStation)
    {
        _dbSet.Remove(chargeStation);
    }

    public async Task<List<ChargeStationEntity>> GetChargeStationsByGroupId(Guid groupId, CancellationToken cancellationToken)
    {
        return await _dbSet.AsQueryable()
            .Where(cs => cs.GroupId == groupId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ChargeStationEntity>> GetAllChargeStations(CancellationToken cancellationToken)
    {
        return await _dbSet.AsQueryable().ToListAsync(cancellationToken);
    }

    public async Task<ChargeStationEntity?> GetChargeStationById(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync(id, cancellationToken);
    }

    public async Task<ChargeStationEntity?> GetChargeStationWithGroupById(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Include(cs => cs.Group)
            .FirstOrDefaultAsync(cs => cs.Id == id, cancellationToken: cancellationToken);
    }

    public async Task<bool> DoesConnectorExist(Guid chargeStationId, int connectorId, CancellationToken cancellationToken)
    {
        return await _dbSet.AsQueryable()
            .AnyAsync(
                cs => cs.Id == chargeStationId && cs.Connectors.Any(c => c.Id == connectorId),
                cancellationToken: cancellationToken);
    }

    public async Task<int> GetConnectorMaxCurrent(Guid chargeStationId, int connectorId, CancellationToken cancellationToken)
    {
        return await _dbSet.AsQueryable()
            .Where(cs => cs.Id == chargeStationId)
            .SelectMany(cs => cs.Connectors)
            .Where(c => c.Id == connectorId)
            .Select(c => c.MaxCurrent)
            .SingleOrDefaultAsync(cancellationToken: cancellationToken);
    }
}