using Bogus;
using FluentAssertions;
using CharginAssignment.WithTests.Application.GroupUseCases.GetAllGroups;
using CharginAssignment.WithTests.Application.GroupUseCases.UpdateGroup;
using CharginAssignment.WithTests.Domain.Constants;
using CharginAssignment.WithTests.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace CharginAssignment.WithTests.Application.IntegrationTests.GroupUseCases;

public class GetAllGroupsIntegrationTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllGroups_WhenCallForFirstTimeWithoutPaginationArgs_ReturnsAllGroupsFromDatabase()
    {
        // Arrange
        List<Guid> addedGroups = new();

        for (int i = 0; i < 10; i++)
        {
            var newGroupId = await TestUtility.CreateGroup(Mediator, TestUtility.FakeName, new Faker().Random.Int(1));
            addedGroups.Add(newGroupId);
        }

        var query = new GetAllGroupsQuery(null, null);

        // Act
        var fetchResult = await Mediator.Send(query);

        // Assert
        fetchResult.Should().HaveCountGreaterOrEqualTo(10);
        addedGroups.Should().OnlyContain(gId => fetchResult.Any(f => f.Id == gId));
    }

    [Theory]
    [InlineData(1, 20, 150)]
    [InlineData(1, 20, 10)]
    [InlineData(3, 20, 150)]
    public async Task GetAllGroups_WhenCallForFirstTimeGivenPaginationArgs_ReturnsCorrectGroupsFromDatabase(int pageNo, int pageSize, int totalCount)
    {
        // Arrange
        await TestUtility.RemoveAllTableRecords<GroupEntity>(DbContext);

        List<Guid> addedGroups = new();

        pageNo = pageNo == 0 ? 1 : pageNo;
        var skip = pageSize * (pageNo - 1);

        for (int i = 0; i < totalCount; i++)
        {
            var newGroupId = await TestUtility.CreateGroup(Mediator, TestUtility.FakeName, new Faker().Random.Int(1));

            addedGroups.Add(newGroupId);
        }

        var expectedGroupIds = addedGroups
            .OrderByDescending(g => g)
            .Skip(skip)
            .Take(pageSize)
            .ToList();

        var query = new GetAllGroupsQuery(pageNo, pageSize);

        // Act
        var fetchResult = await Mediator.Send(query);

        // Assert
        fetchResult.Should().HaveCountLessThanOrEqualTo(pageSize);
        fetchResult.Should().HaveCount(expectedGroupIds.Count);
        fetchResult.Select(f => f.Id).ToList().Should().BeEquivalentTo(expectedGroupIds);
    }

    [Fact]
    public async Task GetAllGroups_ReturnsAllGroupsFromCache()
    {
        // Arrange
        List<Guid> addedGroups = new();

        for (int i = 0; i < 10; i++)
        {
            var newGroupId = await TestUtility.CreateGroup(Mediator, TestUtility.FakeName, new Faker().Random.Int(1));
            addedGroups.Add(newGroupId);
        }

        var query = new GetAllGroupsQuery(null, null);

        // Act
        var cachedResultBeforeCall = Cache.Get<List<GroupEntity>>(CacheKeyConst.AllGroups);
        var callResult = await Mediator.Send(query);
        var cachedResultAfterCall = Cache.Get<List<GroupEntity>>(CacheKeyConst.AllGroups);

        // Assert
        callResult.Should().HaveCountGreaterOrEqualTo(10);
        addedGroups.Should().OnlyContain(gId => callResult.Any(c => c.Id == gId));
        cachedResultBeforeCall.Should().BeNull();
        cachedResultAfterCall.Should().HaveCount(callResult.Count);
        cachedResultAfterCall.Should().BeEquivalentTo(callResult);
    }

    [Fact]
    public async Task GetAllGroups_WhenAGroupAddedOrUpdatedOrDeleted_ReturnsAllGroupsDirectlyFromDatabase()
    {
        // Arrange
        List<Guid> addedGroups = new();

        for (int i = 0; i < 10; i++)
        {
            var newGroupId = await TestUtility.CreateGroup(Mediator, TestUtility.FakeName, new Faker().Random.Int(1));
            addedGroups.Add(newGroupId);
        }

        var query = new GetAllGroupsQuery(null, null);

        // Act
        var cachedResultBeforeInitialCall = Cache.Get<List<GroupEntity>>(CacheKeyConst.AllGroups);
        var initialCallResult = await Mediator.Send(query);
        var cachedResultAfterInitialCall = Cache.Get<List<GroupEntity>>(CacheKeyConst.AllGroups);
        await Mediator.Send(new UpdateGroupCommand(addedGroups.First(), TestUtility.FakeName, new Faker().Random.Int(1)));
        var cachedResultAfterUpdateCall = Cache.Get<List<GroupEntity>>(CacheKeyConst.AllGroups);
        var finalCallResult = await Mediator.Send(query);

        // Assert
        initialCallResult.Should().HaveCountGreaterOrEqualTo(10);
        addedGroups.Should().OnlyContain(gId => initialCallResult.Any(i => i.Id == gId));
        cachedResultBeforeInitialCall.Should().BeNull();
        cachedResultAfterInitialCall.Should().HaveCount(initialCallResult.Count);
        cachedResultAfterUpdateCall.Should().BeNull();
        finalCallResult.Should().NotBeEquivalentTo(initialCallResult);
    }
}