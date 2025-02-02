using CharginAssignment.WithTests.Application.Common.Contracts;
using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Domain.Entities;

namespace CharginAssignment.WithTests.Application.GroupUseCases.CreateGroup;

public record CreateGroupCommand(string? Name, int Capacity) : IRequest<Guid>;

public class CreateGroupCommandHandler(IGroupRepository groupRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateGroupCommand, Guid>
{
    public async Task<Guid> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var newGroupId = Guid.NewGuid();

        var groupToAdd = new GroupEntity
        {
            Id = newGroupId,
            Name = request.Name,
            Capacity = request.Capacity
        };

        await groupRepository.AddGroup(groupToAdd, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return groupToAdd.Id;
    }
}