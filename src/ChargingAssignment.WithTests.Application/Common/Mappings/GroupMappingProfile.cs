using AutoMapper;
using CharginAssignment.WithTests.Application.GroupUseCases.GetAllGroups;
using CharginAssignment.WithTests.Application.GroupUseCases.GetGroupById;
using CharginAssignment.WithTests.Domain.Entities;

namespace CharginAssignment.WithTests.Application.Common.Mappings;

public class GroupMappingProfile : Profile
{
    public GroupMappingProfile()
    {
        CreateMap<GroupEntity, GetGroupByIdQueryResponse>();

        CreateMap<GroupEntity, GetAllGroupsQueryResponseItem>();
    }
}