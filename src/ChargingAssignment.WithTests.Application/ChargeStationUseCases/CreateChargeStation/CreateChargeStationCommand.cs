using AutoMapper;
using CharginAssignment.WithTests.Application.ChargeStationUseCases.GetChargeStationById;
using CharginAssignment.WithTests.Application.Common.Contracts;
using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Application.Common.Exceptions;
using CharginAssignment.WithTests.Domain.Entities;

namespace CharginAssignment.WithTests.Application.ChargeStationUseCases.CreateChargeStation;

public record CreateChargeStationCommand(Guid GroupId, string? Name, List<ConnectorDto>? Connectors) : IRequest<Guid>;

public class CreateChargeStationCommandHandler(
    IGroupRepository groupRepository,
    IChargeStationRepository chargeStationRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IRequestHandler<CreateChargeStationCommand, Guid>
{
    public async Task<Guid> Handle(CreateChargeStationCommand request, CancellationToken cancellationToken)
    {
        // If the GroupId is valid
        var group = await groupRepository.GetGroupById(request.GroupId, cancellationToken);

        if (group is null)
        {
            throw new NotFoundException("Group cannot be found. Check the GroupId, please");
        }

        // The rule check: Group capacity should be greater than or equal to the sum of Max current related Connectors

        var groupMaxCurrentSumBeforeAdding = await groupRepository.SumMaxCurrentOfGroupConnectors(request.GroupId, cancellationToken);

        if (group.Capacity < groupMaxCurrentSumBeforeAdding + request.Connectors!.Sum(c => c.MaxCurrent))
        {
            throw new GroupCapacityExceedsException(group.Capacity, groupMaxCurrentSumBeforeAdding);
        }

        var chargeStationToAdd = mapper.Map<ChargeStationEntity>(request);

        await chargeStationRepository.AddChargeStation(chargeStationToAdd, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return chargeStationToAdd.Id;
    }
}