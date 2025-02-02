using CharginAssignment.WithTests.Application.Common.Contracts;
using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Application.Common.Exceptions;

namespace CharginAssignment.WithTests.Application.GroupUseCases.UpdateGroup;

public record UpdateGroupCommand(Guid GroupId, string? Name, int Capacity) : IRequest;

public class UpdateGroupCommandHandler(IGroupRepository groupRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateGroupCommand>
{
    public async Task Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetGroupById(request.GroupId, cancellationToken);

        if (group is null)
        {
            throw new NotFoundException("No group was found");
        }

        // Check the rule: Capacity should be greater than or equal to the sum of Max current related Connectors

        int sumMaxCurrentOfGroupConnectors = await groupRepository.SumMaxCurrentOfGroupConnectors(group.Id, cancellationToken);

        if (request.Capacity < sumMaxCurrentOfGroupConnectors)
        {
            throw new GroupCapacityExceedsException(group.Capacity, sumMaxCurrentOfGroupConnectors);
        }

        group.Name = request.Name;
        group.Capacity = request.Capacity;

        groupRepository.UpdateGroup(group);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}