using FluentAssertions;
using CharginAssignment.WithTests.Application.GroupUseCases.UpdateGroup;
using CharginAssignment.WithTests.Domain.Entities;

namespace CharginAssignment.WithTests.Application.IntegrationTests.GroupUseCases;

public class UpdateGroupIntegrationTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task UpdateGroup_WhenGivenANewCapacity_ChangesTheGroupCapacityInDatabase()
    {
        // Arrange
        int oldCapacity = 1;
        int newCapacity = 2;

        var newGeneratedGroupId = await TestUtility.CreateGroup(Mediator, TestUtility.FakeName, oldCapacity);

        // Act
        await Mediator.Send(new UpdateGroupCommand(newGeneratedGroupId, TestUtility.FakeName, newCapacity));
        var fetchedGroup = await DbContext.Set<GroupEntity>().FindAsync(newGeneratedGroupId);

        // Assert
        newGeneratedGroupId.Should().NotBeEmpty();
        fetchedGroup.Should().NotBeNull();
        fetchedGroup!.Capacity.Should().Be(newCapacity);
    }
}