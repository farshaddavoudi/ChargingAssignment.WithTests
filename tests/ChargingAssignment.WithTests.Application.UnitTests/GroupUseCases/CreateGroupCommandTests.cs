using CharginAssignment.WithTests.Application.Common.Contracts;
using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Application.GroupUseCases.CreateGroup;

namespace CharginAssignment.WithTests.Application.UnitTests.GroupUseCases;

public class CreateGroupCommandTests
{
    private readonly CreateGroupCommandHandler _handler;
    private readonly CreateGroupCommandValidator _validator = new();
    private readonly Mock<IGroupRepository> _groupRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public CreateGroupCommandTests()
    {
        _handler = new CreateGroupCommandHandler(_groupRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateGroup_WhenGivenValidGroup_AddsNewGroupSuccessfullyWithoutAnyExceptions()
    {
        // Arrange
        var createGroupCommand = new CreateGroupCommand(new Faker().Name.JobArea(), new Faker().Random.Int(1));

        // Act
        Func<Task> func = async () => await _handler.Handle(createGroupCommand, CancellationToken.None);

        // Assert
        await func.Should().NotThrowAsync();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-2)]
    public async Task CreateGroup_WhenGivenGroupWithCapacityLessOrEqualThanZero_FailsWithValidationError(int capacity)
    {
        // Arrange
        var createGroupCommand = new CreateGroupCommand(new Faker().Name.JobArea(), capacity);

        // Act
        var validationResult = await _validator.ValidateAsync(createGroupCommand);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().ContainSingle(e => e.PropertyName.Contains(nameof(CreateGroupCommand.Capacity)));
    }

    [Theory]
    [InlineData("")]
    [InlineData(data: null)]
    public async Task CreateGroup_WhenGivenGroupWithNoName_FailsWithValidationError(string? groupName)
    {
        // Arrange
        var createGroupCommand = new CreateGroupCommand(groupName, new Faker().Random.Int(1));

        // Act
        var validationResult = await _validator.ValidateAsync(createGroupCommand, CancellationToken.None);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().ContainSingle(e => e.PropertyName.Contains(nameof(CreateGroupCommand.Name)));
    }
}