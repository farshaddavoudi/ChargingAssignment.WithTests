using CharginAssignment.WithTests.Application.Common.Contracts;
using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Application.Common.Exceptions;
using CharginAssignment.WithTests.Application.ConnectorUseCases.UpdateConnectorMaxCurrent;
using CharginAssignment.WithTests.Domain.Entities;

namespace CharginAssignment.WithTests.Application.UnitTests.ConnectorUseCases;

public class UpdateConnectorMaxCurrentCommandTests
{
    private readonly UpdateConnectorMaxCurrentCommandHandler _handler;
    private readonly UpdateConnectorMaxCurrentCommandValidator _validator = new();
    private readonly Mock<IGroupRepository> _groupRepositoryMock = new();
    private readonly Mock<IChargeStationRepository> _chargeStationRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public UpdateConnectorMaxCurrentCommandTests()
    {
        _handler = new UpdateConnectorMaxCurrentCommandHandler(
            _chargeStationRepositoryMock.Object,
            _groupRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task UpdateConnectorMaxCurrent_WhenGivenMaxCurrentLessOrEqualZero_ThrowsValidationException()
    {
        // Arrange
        var command = new UpdateConnectorMaxCurrentCommand(Guid.NewGuid(), new Faker().Random.Int(1, 5), 0);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains(nameof(UpdateConnectorMaxCurrentCommand.MaxCurrent)));
        result.Errors.Should().ContainSingle(e => e.ErrorMessage.Contains("greater than zero"));
    }

    [Fact]
    public async Task UpdateConnectorMaxCurrent_WhenGivenChargeStationIdDoesNotExistInDb_ThrowsNotFoundException()
    {
        // Arrange
        _chargeStationRepositoryMock
            .Setup(repo => repo.GetChargeStationWithGroupById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ChargeStationEntity?)null);

        var command = new UpdateConnectorMaxCurrentCommand(Guid.NewGuid(), new Faker().Random.Int(1, 5), new Faker().Random.Int(1));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateConnectorMaxCurrent_WhenGivenConnectorIdDoesNotExistInDb_ThrowsNotFoundException()
    {
        // Arrange
        int connectorIdToUpdate = 1;
        Guid chargeStationId = Guid.NewGuid();

        _chargeStationRepositoryMock
            .Setup(repo => repo.GetChargeStationWithGroupById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChargeStationEntity { Id = chargeStationId, Connectors = new List<ConnectorEntity> { new() { Id = 2 } } });

        var command = new UpdateConnectorMaxCurrentCommand(chargeStationId, connectorIdToUpdate, connectorIdToUpdate);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("No Connector*");
    }

    [Fact]
    public async Task UpdateConnectorMaxCurrent_WhenGivenMaxCurrentCausesToExceedGroupCapacity_ThrowsGroupCapacityExceedsException()
    {
        // Arrange
        int connectorIdToUpdate = 1;
        Guid chargeStationId = Guid.NewGuid();
        int groupCapacity = 10;
        int oldConnectorMaxCurrent = 3;
        int newMaxCurrent = groupCapacity + 1;

        _chargeStationRepositoryMock
            .Setup(repo => repo.GetChargeStationWithGroupById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChargeStationEntity
            {
                Id = chargeStationId,
                Group = new GroupEntity { Capacity = groupCapacity },
                Connectors = new List<ConnectorEntity> { new() { Id = connectorIdToUpdate, MaxCurrent = oldConnectorMaxCurrent } }
            });

        _groupRepositoryMock
            .Setup(x => x.SumMaxCurrentOfGroupConnectors(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(oldConnectorMaxCurrent);

        var command = new UpdateConnectorMaxCurrentCommand(chargeStationId, connectorIdToUpdate, newMaxCurrent);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowExactlyAsync<GroupCapacityExceedsException>();
    }

    [Fact]
    public async Task UpdateConnectorMaxCurrent_WhenRequestIsValid_CallsUnitOfWorkSaveChangesOnce()
    {
        // Arrange
        int connectorIdToUpdate = 1;
        Guid chargeStationId = Guid.NewGuid();

        _chargeStationRepositoryMock
            .Setup(repo => repo.GetChargeStationWithGroupById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChargeStationEntity
            {
                Id = chargeStationId,
                Group = new GroupEntity { Capacity = new Faker().Random.Int(10) },
                Connectors = new List<ConnectorEntity> { new() { Id = connectorIdToUpdate, MaxCurrent = 1 } }
            });

        var command = new UpdateConnectorMaxCurrentCommand(chargeStationId, connectorIdToUpdate, 2);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock
            .Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateConnectorMaxCurrent_WhenRequestIsValid_ChangesTheConnectorMaxCurrentBeforeCallTheRepo()
    {
        // Arrange
        int connectorIdToUpdate = 1;
        int newMaxCurrent = 2;
        Guid chargeStationId = Guid.NewGuid();

        _chargeStationRepositoryMock
            .Setup(repo => repo.GetChargeStationWithGroupById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChargeStationEntity
            {
                Id = chargeStationId,
                Group = new GroupEntity { Capacity = new Faker().Random.Int(10) },
                Connectors = new List<ConnectorEntity> { new() { Id = connectorIdToUpdate, MaxCurrent = 1 } }
            });

        var command = new UpdateConnectorMaxCurrentCommand(chargeStationId, connectorIdToUpdate, newMaxCurrent);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _chargeStationRepositoryMock.Verify(x =>
                x.UpdateChargeStation(It.Is<ChargeStationEntity>(cs =>
                    cs.Connectors.Any(c => c.Id == connectorIdToUpdate && c.MaxCurrent == newMaxCurrent))),
            Times.Once);
    }
}