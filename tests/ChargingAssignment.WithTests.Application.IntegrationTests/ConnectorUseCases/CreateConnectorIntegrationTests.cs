using FluentAssertions;
using CharginAssignment.WithTests.Application.Common.Exceptions;
using CharginAssignment.WithTests.Application.ConnectorUseCases.CreateConnector;

namespace CharginAssignment.WithTests.Application.IntegrationTests.ConnectorUseCases;

public class CreateConnectorIntegrationTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task CreateConnector_WhenGivenValidConnector_AddsConnectorToDatabase()
    {
        // Arrange
        var newGeneratedChargeStationId = await TestUtility.CreateChargeStation(
            Mediator,
            10,
            TestUtility.FakeName,
            (1, 1), (2, 5));

        // Act
        var command = new CreateConnectorCommand(newGeneratedChargeStationId, 3, 1);
        await Mediator.Send(command);

        // Assert
        var fetchedChargeStation = await TestUtility.FetchChargeStationById(DbContext, newGeneratedChargeStationId);

        fetchedChargeStation.Should().NotBeNull();
        fetchedChargeStation!.Connectors.Should().HaveCount(3);
        fetchedChargeStation.Connectors.Should().ContainSingle(c => c.Id == 3 && c.MaxCurrent == 1);
    }

    [Fact]
    public async Task CreateConnector_WhenGivenConnectorMaxCurrentCauseTheGroupCapacityExceeds_ThrowsGroupCapacityExceedsException()
    {
        // Arrange
        var newGeneratedChargeStationId = await TestUtility.CreateChargeStation(
            Mediator,
            10,
            TestUtility.FakeName,
            (1, 1), (2, 5));

        // Act
        var command = new CreateConnectorCommand(newGeneratedChargeStationId, 3, 5);
        Func<Task> act = async () => await Mediator.Send(command);

        // Assert
        await act.Should().ThrowExactlyAsync<GroupCapacityExceedsException>();
    }

    [Fact]
    public async Task CreateConnector_WhenGivenConnectorIdAlreadyExists_ThrowsBusinessLogicException()
    {
        // Arrange
        var newGeneratedChargeStationId = await TestUtility.CreateChargeStation(
            Mediator,
            10,
            TestUtility.FakeName,
            (1, 1), (2, 5));

        // Act
        var command = new CreateConnectorCommand(newGeneratedChargeStationId, 2, 1);
        Func<Task> act = async () => await Mediator.Send(command);

        // Assert
        await act.Should().ThrowAsync<BusinessLogicException>();
    }
}