using FluentAssertions;
using CharginAssignment.WithTests.Application.ConnectorUseCases.UpdateConnectorMaxCurrent;

namespace CharginAssignment.WithTests.Application.IntegrationTests.ConnectorUseCases;

public class UpdateConnectorMaxCurrentIntegrationTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Theory]
    [InlineData(10, 1, 2, 3)]
    [InlineData(10, 8, 1, 2)]
    [InlineData(10, 3, 7, 5)]
    public async Task UpdateConnectorMaxCurrent_WhenGivenNewValidMaxCurrent_ChangesMaxCurrentInDatabase(
        int groupCapacity, int firstConnectorMaxCurrent, int secondConnectorMaxCurrentWhichIsGoingUnderUpdate, int newMaxCurrent)
    {
        // Arrange
        var newGeneratedChargeStationId = await TestUtility.CreateChargeStation(
            Mediator,
            groupCapacity,
            TestUtility.FakeName,
            (1, firstConnectorMaxCurrent), (2, secondConnectorMaxCurrentWhichIsGoingUnderUpdate));

        // Act
        await Mediator.Send(new UpdateConnectorMaxCurrentCommand(newGeneratedChargeStationId, 2, newMaxCurrent));
        var fetchedChargeStation = await TestUtility.FetchChargeStationById(DbContext, newGeneratedChargeStationId);

        // Assert
        newGeneratedChargeStationId.Should().NotBeEmpty();
        fetchedChargeStation.Should().NotBeNull();
        fetchedChargeStation!.Connectors.Should().HaveCount(2);
        fetchedChargeStation.Connectors.Should().ContainSingle(c => c.Id == 2);
        fetchedChargeStation.Connectors.First(c => c.Id == 2).MaxCurrent.Should().Be(newMaxCurrent);
    }
}