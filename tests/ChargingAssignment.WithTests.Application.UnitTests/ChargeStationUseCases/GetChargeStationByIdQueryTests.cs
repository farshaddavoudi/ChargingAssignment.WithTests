using AutoMapper;
using CharginAssignment.WithTests.Application.ChargeStationUseCases.GetChargeStationById;
using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Application.Common.Exceptions;
using CharginAssignment.WithTests.Domain.Entities;

namespace CharginAssignment.WithTests.Application.UnitTests.ChargeStationUseCases;

public class GetChargeStationByIdQueryTests
{
    private readonly GetChargeStationByIdQueryHandler _handler;
    private readonly Mock<IChargeStationRepository> _chargeStationRepositoryMock = new();
    private readonly Mock<IMapper> _mapper = new();

    public GetChargeStationByIdQueryTests()
    {
        _handler = new GetChargeStationByIdQueryHandler(_chargeStationRepositoryMock.Object, _mapper.Object);
    }

    [Fact]
    public async Task GetChargeStationById_WhenGivenChargeStationIdDoesNotExistInDb_ThrowsNotFoundException()
    {
        // Arrange
        _chargeStationRepositoryMock
            .Setup(repo => repo.GetChargeStationById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ChargeStationEntity?)null);

        var query = new GetChargeStationByIdQuery(Guid.NewGuid());

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetChargeStationById_WhenRequestIsValid_ReturnsMappedChargeStationAlongWithItsConnectors()
    {
        // Arrange

        var chargeStationEntity = new ChargeStationEntity
        {
            Id = Guid.NewGuid(),
            GroupId = Guid.NewGuid(),
            Name = "CSName",
            Connectors = new List<ConnectorEntity>
            {
                new() { Id = 1, MaxCurrent = 10 },
                new() { Id = 2, MaxCurrent = 9 }
            }
        };

        var expectedChargeStation = new ChargeStationDto(chargeStationEntity.Id, chargeStationEntity.GroupId,
            chargeStationEntity.Name, [new(1, 10), new(2, 9)]);

        _chargeStationRepositoryMock
            .Setup(repo => repo.GetChargeStationById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(chargeStationEntity);

        _mapper
            .Setup(x => x.Map<ChargeStationDto>(chargeStationEntity))
            .Returns(expectedChargeStation);

        var query = new GetChargeStationByIdQuery(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().Be(expectedChargeStation);
    }
}