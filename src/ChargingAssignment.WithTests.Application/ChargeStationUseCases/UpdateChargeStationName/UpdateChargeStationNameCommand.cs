using CharginAssignment.WithTests.Application.Common.Contracts;
using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Application.Common.Exceptions;

namespace CharginAssignment.WithTests.Application.ChargeStationUseCases.UpdateChargeStationName;

public record UpdateChargeStationNameCommand(Guid ChargeStationId, string? Name) : IRequest;

public class UpdateChargeStationNameCommandHandler(IChargeStationRepository chargeStationRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateChargeStationNameCommand>
{
    public async Task Handle(UpdateChargeStationNameCommand request, CancellationToken cancellationToken)
    {
        var chargeStation = await chargeStationRepository.GetChargeStationById(request.ChargeStationId, cancellationToken);

        if (chargeStation is null)
        {
            throw new NotFoundException("The Charge Station cannot be found");
        }

        chargeStation.Name = request.Name;

        chargeStationRepository.UpdateChargeStation(chargeStation);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}