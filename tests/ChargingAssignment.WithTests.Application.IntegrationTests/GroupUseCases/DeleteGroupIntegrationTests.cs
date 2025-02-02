using FluentAssertions;
using CharginAssignment.WithTests.Application.GroupUseCases.DeleteGroup;

namespace CharginAssignment.WithTests.Application.IntegrationTests.GroupUseCases;

public class DeleteGroupIntegrationTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task DeleteGroup_WhenRequestIsValid_RemovesGroupWithRelatedChargeStations()
    {
        // Arrange
        var result = await TestUtility.CreateGroupWithChargeStation(
            Mediator, 10, TestUtility.FakeName, (1, 1), (2, 3));

        // Act
        var deleteCommand = new DeleteGroupCommand(result.groupId);
        await Mediator.Send(deleteCommand);

        // Assert
        var fetchedChargeStation = await TestUtility.FetchChargeStationById(DbContext, result.ChargeStationId);
        var fetchedGroup = await TestUtility.FetchGroupById(DbContext, result.groupId);

        fetchedChargeStation.Should().BeNull();
        fetchedGroup.Should().BeNull();
    }
}