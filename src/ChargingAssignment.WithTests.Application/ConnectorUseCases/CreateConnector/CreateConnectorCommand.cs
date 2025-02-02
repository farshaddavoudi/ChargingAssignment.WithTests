using CharginAssignment.WithTests.Application.Common.Contracts;
using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Application.Common.Exceptions;
using CharginAssignment.WithTests.Domain.Entities;

namespace CharginAssignment.WithTests.Application.ConnectorUseCases.CreateConnector;

public record CreateConnectorCommand(Guid ChargeStationId, int ConnectorId, int MaxCurrent) : IRequest;

public class CreateConnectorCommandHandler(
    IChargeStationRepository chargeStationRepository,
    IGroupRepository groupRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateConnectorCommand>
{
    public async Task Handle(CreateConnectorCommand request, CancellationToken cancellationToken)
    {
        // Charge Station should be valid
        var chargeStation = await chargeStationRepository.GetChargeStationWithGroupById(request.ChargeStationId, cancellationToken);

        if (chargeStation is null)
        {
            throw new NotFoundException("Charge Station cannot be found");
        }

        // It shouldn't be outside limit of Connectors a Charge Station can have
        if (chargeStation.Connectors.Count >= 5)
        {
            throw new BusinessLogicException("You cannot add any more Connector to this Charge Station due to 5 Connectors limitation");
        }

        // ConnectorId should be unique in context of that Charge Station
        if (chargeStation.Connectors.Any(c => c.Id == request.ConnectorId))
        {
            throw new BusinessLogicException("A Connector with the same ID already exists");
        }

        // The rule check: Group capacity should be greater than or equal to the sum of Max current related Connectors
        // MaxCurrent shouldn't cause to break this rule

        var groupMaxCurrentSumBeforeAdding = await groupRepository.SumMaxCurrentOfGroupConnectors(chargeStation.GroupId, cancellationToken);

        if (chargeStation.Group!.Capacity < groupMaxCurrentSumBeforeAdding + request.MaxCurrent)
        {
            throw new GroupCapacityExceedsException(chargeStation.Group!.Capacity, groupMaxCurrentSumBeforeAdding);
        }

        chargeStation.Connectors.Add(new ConnectorEntity
        {
            Id = request.ConnectorId,
            MaxCurrent = request.MaxCurrent
        });

        chargeStationRepository.UpdateChargeStation(chargeStation);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}