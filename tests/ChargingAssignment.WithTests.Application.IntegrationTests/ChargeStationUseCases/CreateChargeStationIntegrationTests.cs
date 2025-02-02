using FluentAssertions;
using CharginAssignment.WithTests.Application.ChargeStationUseCases.CreateChargeStation;
using CharginAssignment.WithTests.Application.ChargeStationUseCases.GetChargeStationById;
using CharginAssignment.WithTests.Application.Common.Exceptions;

namespace CharginAssignment.WithTests.Application.IntegrationTests.ChargeStationUseCases;

public class CreateChargeStationIntegrationTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task CreateChargeStation_WhenGivenConnectorsWithMaxCurrentsWhichCauseGroupCapacityExceeds_ThrowsGroupCapacityExceedsException()
    {
        // Arrange
        var result = await TestUtility.CreateGroupWithChargeStation(
            Mediator, 10, TestUtility.FakeName,
            (1, 3), (2, 3));

        var connectors = new List<ConnectorDto> { new(3, 3), new(4, 3) };
        var command = new CreateChargeStationCommand(result.groupId, TestUtility.FakeName, connectors);

        // Act
        Func<Task> act = async () => await Mediator.Send(command);

        // Assert
        await act.Should().ThrowExactlyAsync<GroupCapacityExceedsException>();
    }
}