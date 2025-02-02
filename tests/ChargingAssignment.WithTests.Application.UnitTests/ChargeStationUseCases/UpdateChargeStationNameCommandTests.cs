using CharginAssignment.WithTests.Application.ChargeStationUseCases.UpdateChargeStationName;
using CharginAssignment.WithTests.Application.Common.Contracts;
using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Application.Common.Exceptions;
using CharginAssignment.WithTests.Domain.Entities;

namespace CharginAssignment.WithTests.Application.UnitTests.ChargeStationUseCases;

public class UpdateChargeStationNameCommandTests
{
    private readonly UpdateChargeStationNameCommandHandler _handler;
    private readonly Mock<IChargeStationRepository> _chargeStationRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public UpdateChargeStationNameCommandTests()
    {
        _handler = new UpdateChargeStationNameCommandHandler(
            _chargeStationRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task UpdateChargeStationName_WhenGivenChargeStationIdDoesNotExistInDb_ThrowsNotFoundException()
    {
        // Arrange
        _chargeStationRepositoryMock
            .Setup(repo => repo.GetChargeStationById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ChargeStationEntity?)null);

        var command = new UpdateChargeStationNameCommand(Guid.NewGuid(), new Faker().Name.JobTitle());

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateChargeStationName_WhenRequestIsValid_RenamesChargeStationBeforePassesItToRepo()
    {
        // Arrange

        var chargeStationId = Guid.NewGuid();
        var chargeStationOldName = "ChargeStationOldName";
        var chargeStationNewName = "ChargeStationNewName";

        _chargeStationRepositoryMock
            .Setup(repo => repo.GetChargeStationById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChargeStationEntity { Id = chargeStationId, Name = chargeStationOldName });

        var command = new UpdateChargeStationNameCommand(chargeStationId, chargeStationNewName);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _chargeStationRepositoryMock.Verify(
            x =>
                x.UpdateChargeStation(It.Is<ChargeStationEntity>(cs => cs.Name == chargeStationNewName)),
            Times.Once);
    }

    [Fact]
    public async Task UpdateChargeStationName_WhenRequestIsValid_CallsUnitOfWorkSaveChangesOnce()
    {
        // Arrange
        _chargeStationRepositoryMock
            .Setup(repo => repo.GetChargeStationById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChargeStationEntity());

        var command = new UpdateChargeStationNameCommand(Guid.NewGuid(), new Faker().Name.JobTitle());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock
            .Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateChargeStationName_WhenRequestIsValid_DoesNotThrowAnyException()
    {
        // Arrange
        _chargeStationRepositoryMock
            .Setup(repo => repo.GetChargeStationById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChargeStationEntity());

        var command = new UpdateChargeStationNameCommand(Guid.NewGuid(), new Faker().Name.JobTitle());

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }
}