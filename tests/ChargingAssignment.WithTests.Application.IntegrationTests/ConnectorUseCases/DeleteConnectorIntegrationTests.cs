using FluentAssertions;
using CharginAssignment.WithTests.Application.Common.Exceptions;
using CharginAssignment.WithTests.Application.ConnectorUseCases.DeleteConnector;

namespace CharginAssignment.WithTests.Application.IntegrationTests.ConnectorUseCases;

public class DeleteConnectorIntegrationTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task DeleteConnector_WhenGivenValidConnectorToDelete_RemovesConnectorFromDatabase()
    {
        // Arrange
        var newGeneratedChargeStationId = await TestUtility.CreateChargeStation(
            Mediator,
            10,
            TestUtility.FakeName,
            (1, 1), (2, 5));

        // Act
        var command = new DeleteConnectorCommand(newGeneratedChargeStationId, 2);
        await Mediator.Send(command);

        // Assert
        var fetchedChargeStation = await TestUtility.FetchChargeStationById(DbContext, newGeneratedChargeStationId);

        fetchedChargeStation.Should().NotBeNull();
        fetchedChargeStation!.Connectors.Should().HaveCount(1);
        fetchedChargeStation.Connectors.Should().NotContain(c => c.Id == 2);
    }

    [Fact]
    public async Task DeleteConnector_WhenTryingToRemoveTheLastConnectorInAChargeStation_ThrowsBusinessLogicException()
    {
        // Arrange
        var newGeneratedChargeStationId = await TestUtility.CreateChargeStation(
            Mediator,
            10,
            TestUtility.FakeName,
            (1, 1));

        // Act
        var command = new DeleteConnectorCommand(newGeneratedChargeStationId, 1);
        Func<Task> act = async () => await Mediator.Send(command);

        // Assert
        await act.Should().ThrowAsync<BusinessLogicException>();
    }
}