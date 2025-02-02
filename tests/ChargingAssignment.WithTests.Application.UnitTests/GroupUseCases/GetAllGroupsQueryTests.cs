using AutoMapper;
using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Application.GroupUseCases.GetAllGroups;
using CharginAssignment.WithTests.Domain.Entities;

namespace CharginAssignment.WithTests.Application.UnitTests.GroupUseCases;

public class GetAllGroupsQueryTests
{
    [Fact]
    public async Task GetAllGroups_ReturnsCorrectGroups()
    {
        // Arrange
        Mock<IGroupRepository> groupRepositoryMock = new();
        Mock<IMapper> mapperMock = new();

        var handler = new GetAllGroupsQueryHandler(groupRepositoryMock.Object, mapperMock.Object);

        var getAllGroupsQueryResponseItemFaker = new Faker<GetAllGroupsQueryResponseItem>()
            .CustomInstantiator(f => new GetAllGroupsQueryResponseItem(
                f.Random.Guid(),
                f.Name.JobTitle(),
                f.Random.Int(1)
            ));

        var expectedQueryResult = getAllGroupsQueryResponseItemFaker.Generate(3);

        groupRepositoryMock
            .Setup(x => x.GetAllGroups(It.IsAny<int>(), It.IsAny<int>(), CancellationToken.None))
            .ReturnsAsync(new List<GroupEntity>());

        mapperMock
            .Setup(x => x.Map<List<GetAllGroupsQueryResponseItem>>(new List<GroupEntity>()))
            .Returns(expectedQueryResult);

        GetAllGroupsQuery query = new(1, 20);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(expectedQueryResult);
    }
}