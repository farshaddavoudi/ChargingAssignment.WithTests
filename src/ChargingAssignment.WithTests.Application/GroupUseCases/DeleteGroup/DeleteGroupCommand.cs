using CharginAssignment.WithTests.Application.Common.Contracts;
using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Application.Common.Exceptions;

namespace CharginAssignment.WithTests.Application.GroupUseCases.DeleteGroup;

public record DeleteGroupCommand(Guid GroupId) : IRequest;

public class DeleteGroupCommandHandler(IGroupRepository groupRepository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteGroupCommand>
{
    public async Task Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        var groupToDelete = await groupRepository.GetGroupById(request.GroupId, cancellationToken);

        if (groupToDelete is null)
        {
            throw new NotFoundException("No group could be found");
        }

        groupRepository.DeleteGroup(groupToDelete);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}