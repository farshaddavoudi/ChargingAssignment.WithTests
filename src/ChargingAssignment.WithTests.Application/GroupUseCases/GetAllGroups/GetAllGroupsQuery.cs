using AutoMapper;
using CharginAssignment.WithTests.Application.Common.Contracts.Repositories;

namespace CharginAssignment.WithTests.Application.GroupUseCases.GetAllGroups;

public record GetAllGroupsQuery(int? PageNo, int? PageSize) : IRequest<List<GetAllGroupsQueryResponseItem>>;

public class GetAllGroupsQueryHandler(IGroupRepository groupRepository, IMapper mapper) : IRequestHandler<GetAllGroupsQuery, List<GetAllGroupsQueryResponseItem>>
{
    public async Task<List<GetAllGroupsQueryResponseItem>> Handle(GetAllGroupsQuery request, CancellationToken cancellationToken)
    {
        if (request.PageNo.HasValue is false || request.PageSize.HasValue is false)
        {
            request = new GetAllGroupsQuery(PageNo: 1, PageSize: int.MaxValue);
        }

        var groups = await groupRepository.GetAllGroups(request.PageNo!.Value, request.PageSize!.Value, cancellationToken);

        var groupsMappedResult = mapper.Map<List<GetAllGroupsQueryResponseItem>>(groups);

        return groupsMappedResult;
    }
}