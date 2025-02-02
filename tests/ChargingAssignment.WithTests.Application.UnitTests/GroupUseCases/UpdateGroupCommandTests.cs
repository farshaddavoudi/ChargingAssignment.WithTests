using CharginAssignment.WithTests.Application.Common.Contracts;
using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Application.Common.Exceptions;
using CharginAssignment.WithTests.Application.GroupUseCases.UpdateGroup;
using CharginAssignment.WithTests.Domain.Entities;

namespace CharginAssignment.WithTests.Application.UnitTests.GroupUseCases;

public class UpdateGroupCommandTests
{
    private readonly UpdateGroupCommandHandler _handler;
    private readonly UpdateGroupCommandValidator _validator = new();
    private readonly Mock<IGroupRepository> _groupRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public UpdateGroupCommandTests()
    {
        _handler = new UpdateGroupCommandHandler(_groupRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-2)]
    public async Task UpdateGroup_WhenGivenGroupWithCapacityLessOrEqualThanZero_FailsWithValidationError(int capacity)
    {
        // Arrange
        var updateGroupCommand = new UpdateGroupCommand(Guid.NewGuid(), new Faker().Name.JobArea(), capacity);

        // Act
        var validationResult = await _validator.ValidateAsync(updateGroupCommand);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().ContainSingle(e => e.PropertyName.Contains(nameof(updateGroupCommand.Capacity)));
    }

    [Theory]
    [InlineData("")]
    [InlineData(data: null)]
    public async Task UpdateGroup_WhenGivenGroupWithNoName_FailsWithValidationError(string? groupName)
    {
        // Arrange
        var updateGroupCommand = new UpdateGroupCommand(Guid.NewGuid(), groupName, new Faker().Random.Int(1));

        // Act
        var validationResult = await _validator.ValidateAsync(updateGroupCommand, CancellationToken.None);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().ContainSingle(e => e.PropertyName.Contains(nameof(updateGroupCommand.Name)));
    }

    [Fact]
    public async Task UpdateGroup_WhenGivenInvalidGroupId_FailsWithNotFoundException()
    {
        // Arrange
        var command = new UpdateGroupCommand(Guid.NewGuid(), new Faker().Name.JobType(), new Faker().Random.Int(1));

        _groupRepositoryMock
            .Setup(gr => gr.GetGroupById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(GroupEntity));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateGroup_WhenGivenACapacityToUpdateLessThanSumMaxCurrentOfGroupConnectors_ThrowsBusinessLogicException()
    {
        // Arrange
        int givenCapacityToUpdate = new Faker().Random.Int(2);

        var command = new UpdateGroupCommand(Guid.NewGuid(), new Faker().Name.JobType(), givenCapacityToUpdate);

        _groupRepositoryMock
            .Setup(gr => gr.GetGroupById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GroupEntity());

        _groupRepositoryMock
            .Setup(gr => gr.SumMaxCurrentOfGroupConnectors(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(givenCapacityToUpdate + 1);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowExactlyAsync<GroupCapacityExceedsException>();
    }

    [Fact]
    public async Task UpdateGroup_WhenGivenValidUpdateGroupInput_UpdatesGroupSuccessfullyWithoutAnyExceptions()
    {
        // Arrange
        int givenCapacityToUpdate = new Faker().Random.Int(1);

        var command = new UpdateGroupCommand(Guid.NewGuid(), new Faker().Name.JobType(), givenCapacityToUpdate);

        _groupRepositoryMock
            .Setup(gr => gr.GetGroupById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GroupEntity());

        _groupRepositoryMock
            .Setup(gr => gr.SumMaxCurrentOfGroupConnectors(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(givenCapacityToUpdate - 1);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task UpdateGroup_WhenGivenValidUpdateGroupInput_CallsUnitOfWorkSaveChangesOnce()
    {
        // Arrange
        int givenCapacityToUpdate = new Faker().Random.Int(1);

        var command = new UpdateGroupCommand(Guid.NewGuid(), new Faker().Name.JobType(), givenCapacityToUpdate);

        _groupRepositoryMock
            .Setup(gr => gr.GetGroupById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GroupEntity());

        _groupRepositoryMock
            .Setup(gr => gr.SumMaxCurrentOfGroupConnectors(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(givenCapacityToUpdate - 1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}