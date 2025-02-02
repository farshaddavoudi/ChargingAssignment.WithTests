using CharginAssignment.WithTests.Application.Common.Contracts;
using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Application.Common.Exceptions;

namespace CharginAssignment.WithTests.Application.ConnectorUseCases.UpdateConnectorMaxCurrent;

public record UpdateConnectorMaxCurrentCommand(Guid ChargeStationId, int ConnectorId, int MaxCurrent) : IRequest;

public class UpdateConnectorMaxCurrentCommandHandler(
    IChargeStationRepository chargeStationRepository,
    IGroupRepository groupRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateConnectorMaxCurrentCommand>
{
    public async Task Handle(UpdateConnectorMaxCurrentCommand request, CancellationToken cancellationToken)
    {
        // Check the Connector exists
        var chargeStation = await chargeStationRepository.GetChargeStationWithGroupById(request.ChargeStationId, cancellationToken);

        if (chargeStation is null)
        {
            throw new NotFoundException("No Charge Station was found");
        }

        var oldConnector = chargeStation.Connectors.FirstOrDefault(c => c.Id == request.ConnectorId);

        if (oldConnector is null)
        {
            throw new NotFoundException("No Connector was found");
        }

        // The rule check: Group capacity should be greater than or equal to the sum of Max current related Connectors
        // MaxCurrent shouldn't cause to break this rule

        var groupMaxCurrentSumBeforeUpdateWithOldMaxCurrentValue = await groupRepository.SumMaxCurrentOfGroupConnectors(chargeStation.GroupId, cancellationToken);

        if (chargeStation.Group!.Capacity < groupMaxCurrentSumBeforeUpdateWithOldMaxCurrentValue - oldConnector.MaxCurrent + request.MaxCurrent)
        {
            throw new GroupCapacityExceedsException(chargeStation.Group.Capacity, groupMaxCurrentSumBeforeUpdateWithOldMaxCurrentValue);
        }

        oldConnector.MaxCurrent = request.MaxCurrent;

        chargeStationRepository.UpdateChargeStation(chargeStation);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}