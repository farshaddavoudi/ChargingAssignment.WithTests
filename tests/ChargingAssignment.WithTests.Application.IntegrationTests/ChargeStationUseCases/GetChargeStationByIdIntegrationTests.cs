using FluentAssertions;

namespace CharginAssignment.WithTests.Application.IntegrationTests.ChargeStationUseCases;

public class GetChargeStationByIdIntegrationTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetChargeStationById_WhenRequestIsValid_ReturnsCorrectChargeStation()
    {
        // Arrange
        var chargeStationName = TestUtility.FakeName;
        var chargeStationId = await TestUtility.CreateChargeStation(Mediator, 10, chargeStationName, (1, 5));

        // Act
        var fetchedChargeStation = await TestUtility.FetchChargeStationById(DbContext, chargeStationId);

        // Assert
        fetchedChargeStation.Should().NotBeNull();
        fetchedChargeStation!.Name.Should().Be(chargeStationName);
        fetchedChargeStation.Connectors.Should().HaveCount(1);
        fetchedChargeStation.Connectors.Should().Contain(c => c.Id == 1 && c.MaxCurrent == 5);
    }
}