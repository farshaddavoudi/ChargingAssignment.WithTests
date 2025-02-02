using CharginAssignment.WithTests.Application.Common.Contracts;
using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Application.Common.Exceptions;

namespace CharginAssignment.WithTests.Application.ConnectorUseCases.DeleteConnector;

public record DeleteConnectorCommand(Guid ChargeStationId, int ConnectorId) : IRequest;

public class DeleteConnectorCommandHandler(IChargeStationRepository chargeStationRepository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteConnectorCommand>
{
    public async Task Handle(DeleteConnectorCommand request, CancellationToken cancellationToken)
    {
        var chargeStation = await chargeStationRepository.GetChargeStationById(request.ChargeStationId, cancellationToken);

        if (chargeStation is null)
        {
            throw new NotFoundException("Charge Station not found");
        }

        var connectorToRemove = chargeStation.Connectors.FirstOrDefault(c => c.Id == request.ConnectorId);

        if (connectorToRemove is null)
        {
            throw new NotFoundException("No Connector found");
        }

        // Check the rule: It cannot be the last Connector of a Charge Station
        if (chargeStation.Connectors.Count == 1)
        {
            throw new BusinessLogicException("It cannot be removed as it is the last Connector of Charge Station");
        }

        chargeStation.Connectors.Remove(connectorToRemove);

        chargeStationRepository.UpdateChargeStation(chargeStation);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}