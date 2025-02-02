using CharginAssignment.WithTests.Application.ChargeStationUseCases.DeleteChargeStation;
using CharginAssignment.WithTests.Application.Common.Contracts;
using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Application.Common.Exceptions;
using CharginAssignment.WithTests.Domain.Entities;

namespace CharginAssignment.WithTests.Application.UnitTests.ChargeStationUseCases;

public class DeleteChargeStationCommandTests
{
    private readonly DeleteChargeStationCommandHandler _handler;
    private readonly Mock<IChargeStationRepository> _chargeStationRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public DeleteChargeStationCommandTests()
    {
        _handler = new DeleteChargeStationCommandHandler(
            _chargeStationRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task DeleteChargeStation_WhenGivenChargeStationIdDoesNotExistInDb_ThrowsNotFoundException()
    {
        // Arrange
        _chargeStationRepositoryMock
            .Setup(repo => repo.GetChargeStationById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ChargeStationEntity?)null);

        var command = new DeleteChargeStationCommand(Guid.NewGuid());

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteChargeStation_WhenRequestIsValid_CallsUnitOfWorkSaveChangesOnce()
    {
        // Arrange
        _chargeStationRepositoryMock
            .Setup(repo => repo.GetChargeStationById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChargeStationEntity());

        var command = new DeleteChargeStationCommand(Guid.NewGuid());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock
            .Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteChargeStation_WhenRequestIsValid_DoesNotThrowException()
    {
        // Arrange
        _chargeStationRepositoryMock
            .Setup(repo => repo.GetChargeStationById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChargeStationEntity());

        var command = new DeleteChargeStationCommand(Guid.NewGuid());

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }
}