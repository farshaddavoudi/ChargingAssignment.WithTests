using CharginAssignment.WithTests.Application.Common.Contracts;
using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Application.Common.Exceptions;
using CharginAssignment.WithTests.Application.ConnectorUseCases.CreateConnector;
using CharginAssignment.WithTests.Domain.Entities;

namespace CharginAssignment.WithTests.Application.UnitTests.ConnectorUseCases;

public class CreateConnectorCommandTests
{
    private readonly CreateConnectorCommandHandler _handler;
    private readonly CreateConnectorCommandValidator _validator = new();
    private readonly Mock<IGroupRepository> _groupRepositoryMock = new();
    private readonly Mock<IChargeStationRepository> _chargeStationRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public CreateConnectorCommandTests()
    {
        _handler = new CreateConnectorCommandHandler(
            _chargeStationRepositoryMock.Object,
            _groupRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateConnector_WhenMaxCurrentIsNotGreaterThanZero_ThrowsValidationException()
    {
        // Arrange
        var command = new CreateConnectorCommand(Guid.NewGuid(), new Faker().Random.Int(1, 5), 0);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains(nameof(CreateConnectorCommand.MaxCurrent)));
        result.Errors.Should().ContainSingle(e => e.ErrorMessage.Contains("greater than zero"));
    }

    [Theory]
    [InlineData(-2)]
    [InlineData(0)]
    [InlineData(6)]
    public async Task CreateConnector_WhenConnectorIdIsNotBetween1And5_ThrowsValidationException(int connectorId)
    {
        // Arrange
        var command = new CreateConnectorCommand(Guid.NewGuid(), connectorId, new Faker().Random.Int(1));

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains(nameof(CreateConnectorCommand.ConnectorId)));
        result.Errors.Should().ContainSingle(e => e.ErrorMessage.Contains("Connector ID"));

    }

    [Fact]
    public async Task CreateConnector_WhenGivenChargeStationDoesNotExistInDb_ThrowsNotFoundException()
    {
        // Arrange
        _chargeStationRepositoryMock
            .Setup(repo => repo.GetChargeStationWithGroupById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ChargeStationEntity?)null);

        var command = new CreateConnectorCommand(Guid.NewGuid(), new Faker().Random.Int(1, 5), new Faker().Random.Int(1));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateConnector_WhenAddingConnectorToChargeStationWithAlready5Connectors_ThrowsLogicException()
    {
        // Arrange
        Guid chargeStationId = Guid.NewGuid();

        _chargeStationRepositoryMock
            .Setup(repo => repo.GetChargeStationWithGroupById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChargeStationEntity { Id = chargeStationId, Connectors = new List<ConnectorEntity> { new(), new(), new(), new(), new() } });

        var command = new CreateConnectorCommand(chargeStationId, new Faker().Random.Int(1, 5), new Faker().Random.Int(1));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowExactlyAsync<BusinessLogicException>()
            .WithMessage("*due to 5 Connectors limitation");
    }

    [Fact]
    public async Task CreateConnector_WhenGivenConnectorHasIdWhichAlreadyExists_ThrowsLogicException()
    {
        // Arrange
        Guid chargeStationId = Guid.NewGuid();
        int connectorId = new Faker().Random.Int(1, 5);

        _chargeStationRepositoryMock
            .Setup(repo => repo.GetChargeStationWithGroupById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChargeStationEntity
            {
                Id = chargeStationId,
                Connectors = new List<ConnectorEntity> { new() { Id = connectorId, MaxCurrent = new Faker().Random.Int(1) } }
            });

        var command = new CreateConnectorCommand(chargeStationId, connectorId, new Faker().Random.Int(1));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowExactlyAsync<BusinessLogicException>()
            .WithMessage("*ID already exists");
    }

    [Fact]
    public async Task CreateConnector_WhenGivenConnectorHasMaxCurrentThatWhenAddedExceedsGroupCapacity_ThrowsGroupCapacityExceedsException()
    {
        // Arrange
        Guid chargeStationId = Guid.NewGuid();
        int groupCapacity = 10;
        int oldConnectorMaxCurrent = 3;
        int newConnectorMaxCurrent = groupCapacity - oldConnectorMaxCurrent + 1; //Add 1 to throw error
        int connectorId = 1;

        _chargeStationRepositoryMock
            .Setup(repo => repo.GetChargeStationWithGroupById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChargeStationEntity
            {
                Id = chargeStationId,
                Group = new GroupEntity { Capacity = groupCapacity },
                Connectors = new List<ConnectorEntity> { new() { Id = connectorId, MaxCurrent = oldConnectorMaxCurrent } }
            });

        _groupRepositoryMock
            .Setup(x => x.SumMaxCurrentOfGroupConnectors(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(oldConnectorMaxCurrent);

        var command = new CreateConnectorCommand(chargeStationId, connectorId + 1, newConnectorMaxCurrent);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowExactlyAsync<GroupCapacityExceedsException>();
    }

    [Fact]
    public async Task CreateConnector_WhenRequestIsValid_AddsToChargeStationConnectorsCount()
    {
        // Arrange
        Guid chargeStationId = Guid.NewGuid();
        int groupCapacity = 10;
        int oldConnectorMaxCurrent = 3;
        int newConnectorMaxCurrent = groupCapacity - oldConnectorMaxCurrent;
        int connectorId = 1;

        _chargeStationRepositoryMock
            .Setup(repo => repo.GetChargeStationWithGroupById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChargeStationEntity
            {
                Id = chargeStationId,
                Group = new GroupEntity { Capacity = groupCapacity },
                Connectors = new List<ConnectorEntity> { new() { Id = connectorId, MaxCurrent = oldConnectorMaxCurrent } }
            });

        _groupRepositoryMock
            .Setup(x => x.SumMaxCurrentOfGroupConnectors(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(oldConnectorMaxCurrent);

        var command = new CreateConnectorCommand(chargeStationId, connectorId + 1, newConnectorMaxCurrent);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _chargeStationRepositoryMock
                .Verify(x =>
                    x.UpdateChargeStation(It.Is<ChargeStationEntity>(cs => cs.Connectors.Count == 2)),
                    Times.Once);
    }

    [Fact]
    public async Task CreateConnector_WhenRequestIsValid_CallsUnitOfWorkSaveChangesOnce()
    {
        // Arrange
        Guid chargeStationId = Guid.NewGuid();
        int groupCapacity = 10;
        int oldConnectorMaxCurrent = 3;
        int newConnectorMaxCurrent = groupCapacity - oldConnectorMaxCurrent;
        int connectorId = 1;

        _chargeStationRepositoryMock
            .Setup(repo => repo.GetChargeStationWithGroupById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChargeStationEntity
            {
                Id = chargeStationId,
                Group = new GroupEntity { Capacity = groupCapacity },
                Connectors = new List<ConnectorEntity> { new() { Id = connectorId, MaxCurrent = oldConnectorMaxCurrent } }
            });

        _groupRepositoryMock
            .Setup(x => x.SumMaxCurrentOfGroupConnectors(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(oldConnectorMaxCurrent);

        var command = new CreateConnectorCommand(chargeStationId, connectorId + 1, newConnectorMaxCurrent);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock
            .Verify(x =>
                    x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
    }
}