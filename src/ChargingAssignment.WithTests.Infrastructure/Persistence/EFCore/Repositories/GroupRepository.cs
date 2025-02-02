using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Domain.Constants;
using CharginAssignment.WithTests.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CharginAssignment.WithTests.Infrastructure.Persistence.EFCore.Repositories;

public class GroupRepository(AppDbContext dbContext, IMemoryCache cache) : IGroupRepository
{
    private readonly DbSet<GroupEntity> _dbSet = dbContext.Set<GroupEntity>();

    public async Task AddGroup(GroupEntity group, CancellationToken cancellationToken)
    {
        await _dbSet.AddAsync(group, cancellationToken);

        cache.Remove(CacheKeyConst.AllGroups);
    }

    public void UpdateGroup(GroupEntity group)
    {
        _dbSet.Update(group);

        cache.Remove(CacheKeyConst.AllGroups);
    }

    public void DeleteGroup(GroupEntity group)
    {
        _dbSet.Remove(group);

        cache.Remove(CacheKeyConst.AllGroups);
    }

    public async Task<List<GroupEntity>> GetAllGroups(int pageNo, int pageSize, CancellationToken cancellationToken)
    {
        pageNo = pageNo == 0 ? 1 : pageNo;
        var skip = pageSize * (pageNo - 1);

        var allCachedGroups = cache.Get<List<GroupEntity>>(CacheKeyConst.AllGroups);

        if (allCachedGroups is not null)
        {
            return allCachedGroups
                .Skip(skip)
                .Take(pageSize)
                .ToList();
        }

        var allFetchedGroups = await _dbSet.AsQueryable().ToListAsync(cancellationToken);

        cache.Set(CacheKeyConst.AllGroups, allFetchedGroups, TimeSpan.FromDays(1));

        return allFetchedGroups
            .OrderByDescending(g => g.Id)
            .Skip(skip)
            .Take(pageSize)
            .ToList();
    }

    public async Task<GroupEntity?> GetGroupById(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync(id, cancellationToken);
    }

    public async Task<int> SumMaxCurrentOfGroupConnectors(Guid groupId, CancellationToken cancellationToken)
    {
        var sum = await _dbSet.AsQueryable()
            .Where(g => g.Id == groupId)
            .SelectMany(g => g.ChargeStations.SelectMany(cs => cs.Connectors))
            .SumAsync(c => c.MaxCurrent, cancellationToken);

        return sum;
    }
}