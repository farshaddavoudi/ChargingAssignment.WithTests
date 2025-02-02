using Bogus;
using FluentAssertions;

namespace CharginAssignment.WithTests.Application.IntegrationTests.GroupUseCases;

public class CreateGroupIntegrationTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task CreateGroup_WhenRequestIsValid_AddsGroupToDatabase()
    {
        // Arrange & Act
        var newGroupId = await TestUtility.CreateGroup(Mediator, TestUtility.FakeName, new Faker().Random.Int(1));
        var fetchedGroup = await TestUtility.FetchGroupById(DbContext, newGroupId);

        // Assert
        newGroupId.Should().NotBeEmpty();
        fetchedGroup.Should().NotBeNull();
        fetchedGroup?.Id.Should().Be(newGroupId);
    }
}