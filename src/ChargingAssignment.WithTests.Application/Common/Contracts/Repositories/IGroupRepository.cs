using CharginAssignment.WithTests.Domain.Entities;

namespace CharginAssignment.WithTests.Application.Common.Contracts.Repositories;

public interface IGroupRepository
{
    Task AddGroup(GroupEntity group, CancellationToken cancellationToken);

    void UpdateGroup(GroupEntity group);

    void DeleteGroup(GroupEntity group);

    Task<List<GroupEntity>> GetAllGroups(int pageNo, int pageSize, CancellationToken cancellationToken);

    Task<GroupEntity?> GetGroupById(Guid id, CancellationToken cancellationToken);

    Task<int> SumMaxCurrentOfGroupConnectors(Guid groupId, CancellationToken cancellationToken);
}