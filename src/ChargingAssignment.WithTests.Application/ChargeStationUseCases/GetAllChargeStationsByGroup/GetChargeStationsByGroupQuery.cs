using AutoMapper;
using CharginAssignment.WithTests.Application.ChargeStationUseCases.GetChargeStationById;
using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;

namespace CharginAssignment.WithTests.Application.ChargeStationUseCases.GetAllChargeStationsByGroup;

public record GetChargeStationsByGroupQuery(Guid GroupId) : IRequest<List<ChargeStationDto>>;

public class GetAllChargeStationsByGroupQueryHandler(IChargeStationRepository chargeStationRepository, IMapper mapper) : IRequestHandler<GetChargeStationsByGroupQuery, List<ChargeStationDto>>
{
    public async Task<List<ChargeStationDto>> Handle(GetChargeStationsByGroupQuery request, CancellationToken cancellationToken)
    {
        var fetchedChargeStations = await chargeStationRepository.GetChargeStationsByGroupId(request.GroupId, cancellationToken);

        var chargeStations = mapper.Map<List<ChargeStationDto>>(fetchedChargeStations);

        return chargeStations;
    }
}