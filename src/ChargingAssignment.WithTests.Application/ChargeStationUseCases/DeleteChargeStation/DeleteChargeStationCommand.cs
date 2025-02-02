using CharginAssignment.WithTests.Application.Common.Contracts;
using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Application.Common.Exceptions;

namespace CharginAssignment.WithTests.Application.ChargeStationUseCases.DeleteChargeStation;

public record DeleteChargeStationCommand(Guid ChargeStationId) : IRequest;

public class DeleteChargeStationCommandHandler(IChargeStationRepository chargeStationRepository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteChargeStationCommand>
{
    public async Task Handle(DeleteChargeStationCommand request, CancellationToken cancellationToken)
    {
        var chargeStationToDelete = await chargeStationRepository.GetChargeStationById(request.ChargeStationId, cancellationToken);

        if (chargeStationToDelete is null)
        {
            throw new NotFoundException("Charge Station not found");
        }

        chargeStationRepository.DeleteChargeStation(chargeStationToDelete);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}