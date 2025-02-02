using FluentAssertions;
using CharginAssignment.WithTests.Application.ConnectorUseCases.GetConnectorMaxCurrent;

namespace CharginAssignment.WithTests.Application.IntegrationTests.ConnectorUseCases;

public class GetConnectorMaxCurrentIntegrationTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetConnectorMaxCurrent_WhenRequestIsValid_ReturnsCorrectMaxCurrent()
    {
        // Arrange
        var newChargeStationId = await TestUtility.CreateChargeStation(Mediator, 10, TestUtility.FakeName, (1, 5), (3, 2));

        var query = new GetConnectorMaxCurrentQuery(newChargeStationId, 3);

        // Act
        var result = await Mediator.Send(query);

        // Assert
        result.Should().Be(2);
    }
}