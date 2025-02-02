using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Application.Common.Exceptions;

namespace CharginAssignment.WithTests.Application.ConnectorUseCases.GetConnectorMaxCurrent;

public record GetConnectorMaxCurrentQuery(Guid ChargeStationId, int ConnectorId) : IRequest<int>;

public class GetConnectorMaxCurrentQueryHandler(IChargeStationRepository chargeStationRepository) : IRequestHandler<GetConnectorMaxCurrentQuery, int>
{
    public async Task<int> Handle(GetConnectorMaxCurrentQuery request, CancellationToken cancellationToken)
    {
        var doesConnectorExist = await chargeStationRepository.DoesConnectorExist(request.ChargeStationId, request.ConnectorId, cancellationToken);

        if (doesConnectorExist is false)
        {
            throw new NotFoundException("No Connector was found");
        }

        var maxCurrent = await chargeStationRepository.GetConnectorMaxCurrent(request.ChargeStationId, request.ConnectorId, cancellationToken);

        return maxCurrent;
    }
}