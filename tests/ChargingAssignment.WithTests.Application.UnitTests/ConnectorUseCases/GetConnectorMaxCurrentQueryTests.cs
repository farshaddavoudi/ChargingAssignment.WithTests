using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Application.Common.Exceptions;
using CharginAssignment.WithTests.Application.ConnectorUseCases.GetConnectorMaxCurrent;

namespace CharginAssignment.WithTests.Application.UnitTests.ConnectorUseCases;

public class GetConnectorMaxCurrentQueryTests
{
    private readonly GetConnectorMaxCurrentQueryHandler _handler;
    private readonly Mock<IChargeStationRepository> _chargeStationRepositoryMock = new();

    public GetConnectorMaxCurrentQueryTests()
    {
        _handler = new GetConnectorMaxCurrentQueryHandler(_chargeStationRepositoryMock.Object);
    }

    [Fact]
    public async Task GetConnectorMaxCurrent_WhenConnectorDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        _chargeStationRepositoryMock
            .Setup(repo => repo.DoesConnectorExist(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var query = new GetConnectorMaxCurrentQuery(Guid.NewGuid(), new Faker().Random.Int(1, 5));

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetConnectorMaxCurrent_WhenRequestIsValid_ReturnsCorrectMaxCurrent()
    {
        // Arrange
        int expectedMaxCurrent = new Faker().Random.Int(1);

        _chargeStationRepositoryMock
            .Setup(repo => repo.DoesConnectorExist(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _chargeStationRepositoryMock
            .Setup(x => x.GetConnectorMaxCurrent(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMaxCurrent);

        var query = new GetConnectorMaxCurrentQuery(Guid.NewGuid(), new Faker().Random.Int(1, 5));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().Be(expectedMaxCurrent);
    }
}