using AutoMapper;
using CharginAssignment.WithTests.Application.ChargeStationUseCases.CreateChargeStation;
using CharginAssignment.WithTests.Application.ChargeStationUseCases.GetChargeStationById;
using CharginAssignment.WithTests.Application.Common.Contracts;
using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Application.Common.Exceptions;
using CharginAssignment.WithTests.Domain.Entities;

namespace CharginAssignment.WithTests.Application.UnitTests.ChargeStationUseCases;

public class CreateChargeStationCommandTests
{
    private readonly CreateChargeStationCommandHandler _handler;
    private readonly CreateChargeStationCommandValidator _validator = new();
    private readonly Mock<IGroupRepository> _groupRepositoryMock = new();
    private readonly Mock<IChargeStationRepository> _chargeStationRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public CreateChargeStationCommandTests()
    {
        _handler = new CreateChargeStationCommandHandler(
            _groupRepositoryMock.Object,
            _chargeStationRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task CreateChargeStation_WhenGivenChargeStationWithoutAnyConnectors_ThrowsValidationException()
    {
        // Arrange
        var command = new CreateChargeStationCommand(Guid.NewGuid(), new Faker().Name.JobTitle(), new List<ConnectorDto>());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName.Contains(nameof(CreateChargeStationCommand.Connectors)));
    }

    [Fact]
    public async Task CreateChargeStation_WhenGivenChargeStationWithMoreThan5Connectors_ThrowsValidationException()
    {
        // Arrange
        var connectorFaker = new Faker<ConnectorDto>()
            .CustomInstantiator(f => new ConnectorDto(f.Random.Int(1, 5), f.Random.Int(1)));

        var command = new CreateChargeStationCommand(Guid.NewGuid(), new Faker().Name.JobTitle(), connectorFaker.Generate(6));

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains(nameof(CreateChargeStationCommand.Connectors)));
        result.Errors.Should().ContainSingle(e => e.ErrorMessage.Contains("5"));
    }

    [Fact]
    public async Task CreateChargeStation_WhenGivenChargeStationWithConnectorsWhichSomeAreInvalid_ThrowsValidationException()
    {
        // Arrange
        var connectorFaker = new Faker<ConnectorDto>()
            .CustomInstantiator(f => new ConnectorDto(f.Random.Int(6, 8), f.Random.Int(1)));

        var command = new CreateChargeStationCommand(Guid.NewGuid(), new Faker().Name.JobTitle(), connectorFaker.Generate(2));

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains(nameof(CreateChargeStationCommand.Connectors)));
        result.Errors.Should().ContainSingle(e => e.ErrorMessage.Contains("isn't valid"));
    }

    [Fact]
    public async Task CreateChargeStation_WhenGivenChargeStationWithConnectorsHavingDuplicatedIds_ThrowsValidationException()
    {
        // Arrange
        var connectorFaker = new Faker<ConnectorDto>()
            .CustomInstantiator(f => new ConnectorDto(f.Random.Int(1, 4), f.Random.Int(1)));

        var command = new CreateChargeStationCommand(Guid.NewGuid(), new Faker().Name.JobTitle(), connectorFaker.Generate(5));

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains(nameof(CreateChargeStationCommand.Connectors)));
        result.Errors.Should().ContainSingle(e => e.ErrorMessage.Contains("unique"));
    }

    [Fact]
    public async Task CreateChargeStation_WhenGivenGroupIdIsInvalid_ThrowsNotFoundException()
    {
        // Arrange
        _groupRepositoryMock
            .Setup(repo => repo.GetGroupById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GroupEntity?)null);

        var command = new CreateChargeStationCommand(Guid.NewGuid(), new Faker().Name.JobTitle(), new List<ConnectorDto> { new(1, 10) });

        // Act 
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateChargeStation_WhenGivenConnectorsWithMaxCurrentsSumExceedGroupCapacity_ThrowsGroupCapacityExceedsException()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        int groupCapacity = 10;

        _groupRepositoryMock
            .Setup(repo => repo.GetGroupById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GroupEntity { Id = groupId, Capacity = groupCapacity });

        _groupRepositoryMock
            .Setup(gr => gr.SumMaxCurrentOfGroupConnectors(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(groupCapacity + 1);

        var command = new CreateChargeStationCommand(groupId, new Faker().Name.JobTitle(),
            new List<ConnectorDto> { new(1, 1) });

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowExactlyAsync<GroupCapacityExceedsException>();
    }

    [Fact]
    public async Task CreateChargeStation_WhenRequestIsValid_CallsUnitOfWorkSaveChangesOnce()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        int groupCapacity = 10;

        _groupRepositoryMock
            .Setup(repo => repo.GetGroupById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GroupEntity { Id = groupId, Capacity = groupCapacity });

        _groupRepositoryMock
            .Setup(gr => gr.SumMaxCurrentOfGroupConnectors(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(groupCapacity - 1);

        _mapperMock
            .Setup(m => m.Map<ChargeStationEntity>(It.IsAny<CreateChargeStationCommand>()))
            .Returns(new ChargeStationEntity { Id = Guid.NewGuid() });

        var command = new CreateChargeStationCommand(groupId, new Faker().Name.JobTitle(),
            new List<ConnectorDto> { new(1, 1) });

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateChargeStation_WhenRequestIsValid_DoesNotThrowException()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        int groupCapacity = 10;

        _groupRepositoryMock
            .Setup(repo => repo.GetGroupById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GroupEntity { Id = groupId, Capacity = groupCapacity });

        _groupRepositoryMock
            .Setup(gr => gr.SumMaxCurrentOfGroupConnectors(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(groupCapacity - 1);

        _mapperMock
            .Setup(m => m.Map<ChargeStationEntity>(It.IsAny<CreateChargeStationCommand>()))
            .Returns(new ChargeStationEntity { Id = Guid.NewGuid() });

        var command = new CreateChargeStationCommand(groupId, new Faker().Name.JobTitle(),
            new List<ConnectorDto> { new(1, 1) });

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }
}