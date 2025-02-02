using CharginAssignment.WithTests.Application.Common.Contracts;
using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Application.Common.Exceptions;
using CharginAssignment.WithTests.Application.GroupUseCases.DeleteGroup;
using CharginAssignment.WithTests.Domain.Entities;

namespace CharginAssignment.WithTests.Application.UnitTests.GroupUseCases;

public class DeleteGroupCommandTests
{
    private readonly DeleteGroupCommandHandler _handler;
    private readonly Mock<IGroupRepository> _groupRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public DeleteGroupCommandTests()
    {
        _handler = new DeleteGroupCommandHandler(_groupRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task DeleteGroup_WhenGivenInvalidGroupId_FailsWithNotFoundException()
    {
        // Arrange
        var command = new DeleteGroupCommand(Guid.NewGuid());

        _groupRepositoryMock
            .Setup(gr => gr.GetGroupById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(GroupEntity));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteGroup_WhenGivenValidGroupId_CallsUnitOfWorkSaveChangesOnce()
    {
        // Arrange
        var command = new DeleteGroupCommand(Guid.NewGuid());

        _groupRepositoryMock
            .Setup(gr => gr.GetGroupById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GroupEntity());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

}