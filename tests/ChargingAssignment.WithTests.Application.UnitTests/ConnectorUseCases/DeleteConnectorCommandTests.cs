using CharginAssignment.WithTests.Application.Common.Contracts;
using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Application.Common.Exceptions;
using CharginAssignment.WithTests.Application.ConnectorUseCases.DeleteConnector;
using CharginAssignment.WithTests.Domain.Entities;

namespace CharginAssignment.WithTests.Application.UnitTests.ConnectorUseCases;

public class DeleteConnectorCommandTests
{
    private readonly DeleteConnectorCommandHandler _handler;
    private readonly Mock<IChargeStationRepository> _chargeStationRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public DeleteConnectorCommandTests()
    {
        _handler = new DeleteConnectorCommandHandler(
            _chargeStationRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task DeleteConnector_WhenGivenChargeStationIdDoesNotExistInDb_ThrowsNotFoundException()
    {
        // Arrange
        _chargeStationRepositoryMock
            .Setup(repo => repo.GetChargeStationById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ChargeStationEntity?)null);

        var command = new DeleteConnectorCommand(Guid.NewGuid(), new Faker().Random.Int(1, 5));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Charge Station*");
    }

    [Fact]
    public async Task DeleteConnector_WhenGivenConnectorIdDoesNotExistInDb_ThrowsNotFoundException()
    {
        // Arrange
        int connectorIdToRemove = 1;

        _chargeStationRepositoryMock
            .Setup(repo => repo.GetChargeStationById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChargeStationEntity { Connectors = new List<ConnectorEntity> { new() { Id = 2 } } });

        var command = new DeleteConnectorCommand(Guid.NewGuid(), connectorIdToRemove);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("No Connector*");
    }

    [Fact]
    public async Task DeleteConnector_WhenGivenConnectorIsTheLastConnectorForChargeStation_ThrowsLogicException()
    {
        // Arrange
        int connectorIdToRemove = 2;

        _chargeStationRepositoryMock
            .Setup(repo => repo.GetChargeStationById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChargeStationEntity { Connectors = new List<ConnectorEntity> { new() { Id = connectorIdToRemove } } });

        var command = new DeleteConnectorCommand(Guid.NewGuid(), connectorIdToRemove);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowExactlyAsync<BusinessLogicException>().WithMessage("*cannot be removed*");

    }

    [Fact]
    public async Task DeleteConnector_WhenRequestIsValid_CallsUnitOfWorkSaveChangesOnce()
    {
        // Arrange
        int connectorIdToRemove = 2;

        _chargeStationRepositoryMock
            .Setup(repo => repo.GetChargeStationById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChargeStationEntity { Connectors = new List<ConnectorEntity> { new() { Id = 2 }, new() { Id = 3 } } });

        var command = new DeleteConnectorCommand(Guid.NewGuid(), connectorIdToRemove);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock
            .Verify(x =>
                    x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    public async Task DeleteConnector_WhenRequestIsValid_DoesNotThrowException()
    {
        // Arrange
        int connectorIdToRemove = 2;

        _chargeStationRepositoryMock
            .Setup(repo => repo.GetChargeStationById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChargeStationEntity { Connectors = new List<ConnectorEntity> { new() { Id = 2 }, new() { Id = 3 } } });

        var command = new DeleteConnectorCommand(Guid.NewGuid(), connectorIdToRemove);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }
}