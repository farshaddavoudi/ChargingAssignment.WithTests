using AutoMapper;
using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;
using CharginAssignment.WithTests.Application.Common.Exceptions;

namespace CharginAssignment.WithTests.Application.GroupUseCases.GetGroupById;

public record GetGroupByIdQuery(Guid Id) : IRequest<GetGroupByIdQueryResponse>;

public class GetGroupByIdQueryHandler(IGroupRepository groupRepository, IMapper mapper) : IRequestHandler<GetGroupByIdQuery, GetGroupByIdQueryResponse>
{
    public async Task<GetGroupByIdQueryResponse> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetGroupById(request.Id, cancellationToken);

        if (group is null)
        {
            throw new NotFoundException("No group was found");
        }

        var groupMappedResult = mapper.Map<GetGroupByIdQueryResponse>(group);

        return groupMappedResult;
    }
}