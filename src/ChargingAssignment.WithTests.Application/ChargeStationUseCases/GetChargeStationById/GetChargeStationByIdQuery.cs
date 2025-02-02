using AutoMapper;
using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Application.Common.Exceptions;

namespace CharginAssignment.WithTests.Application.ChargeStationUseCases.GetChargeStationById;

public record GetChargeStationByIdQuery(Guid Id) : IRequest<ChargeStationDto>;

public class GetChargeStationByIdQueryHandler(IChargeStationRepository chargeStationRepository, IMapper mapper) : IRequestHandler<GetChargeStationByIdQuery, ChargeStationDto>
{
    public async Task<ChargeStationDto> Handle(GetChargeStationByIdQuery request, CancellationToken cancellationToken)
    {
        var chargeStationEntity = await chargeStationRepository.GetChargeStationById(request.Id, cancellationToken);

        if (chargeStationEntity is null)
        {
            throw new NotFoundException("No Charge Station exists");
        }

        var chargeStation = mapper.Map<ChargeStationDto>(chargeStationEntity);

        return chargeStation;
    }
}

