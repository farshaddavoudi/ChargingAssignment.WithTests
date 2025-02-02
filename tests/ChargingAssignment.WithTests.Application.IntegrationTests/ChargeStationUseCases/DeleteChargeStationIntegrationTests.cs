using FluentAssertions;
using CharginAssignment.WithTests.Application.ChargeStationUseCases.DeleteChargeStation;

namespace CharginAssignment.WithTests.Application.IntegrationTests.ChargeStationUseCases;

public class DeleteChargeStationIntegrationTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task DeleteChargeStation_WhenRequestIsValid_RemovesChargeStationFromDatabase()
    {
        // Arrange
        var newChargeStationId = await TestUtility.CreateChargeStation(Mediator, 10, TestUtility.FakeName, (1, 1));

        // Act
        var command = new DeleteChargeStationCommand(newChargeStationId);
        await Mediator.Send(command);

        // Assert
        var fetchedChargeStation = await TestUtility.FetchChargeStationById(DbContext, newChargeStationId);

        fetchedChargeStation.Should().BeNull();
    }
}