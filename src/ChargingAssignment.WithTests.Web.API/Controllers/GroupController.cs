using CharginAssignment.WithTests.Application.GroupUseCases.CreateGroup;
using CharginAssignment.WithTests.Application.GroupUseCases.DeleteGroup;
using CharginAssignment.WithTests.Application.GroupUseCases.GetAllGroups;
using CharginAssignment.WithTests.Application.GroupUseCases.GetGroupById;
using CharginAssignment.WithTests.Application.GroupUseCases.UpdateGroup;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;

namespace CharginAssignment.WithTests.Web.API.Controllers;

public class GroupController(IMediator mediator) : BaseApiController
{
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status201Created, CreatedDescription, typeof(Guid), JsonContentType)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateGroupCommand group, CancellationToken cancellationToken)
    {
        var newGroupId = await mediator.Send(group, cancellationToken);

        return Created(newGroupId);
    }

    [HttpPut]
    [SwaggerResponse(StatusCodes.Status204NoContent, UpdatedDescription)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, BusinessLogicDescription)]
    public async Task<IActionResult> Update(UpdateGroupCommand command, CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{groupId}")]
    [SwaggerResponse(StatusCodes.Status204NoContent, DeletedDescription)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid groupId, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteGroupCommand(groupId), cancellationToken);

        return NoContent();
    }

    [HttpGet("{id}")]
    [SwaggerResponse(StatusCodes.Status200OK, FetchedDescription, typeof(GetGroupByIdQueryResponse), JsonContentType)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGroupById(Guid id, CancellationToken cancellationToken)
    {
        var group = await mediator.Send(new GetGroupByIdQuery(id), cancellationToken);

        return Ok(group);
    }

    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, FetchedDescription, typeof(List<GetAllGroupsQueryResponseItem>), JsonContentType)]
    public async Task<IActionResult> GetAllGroups([FromQuery] GetAllGroupsQuery query, CancellationToken cancellationToken)
    {
        var groups = await mediator.Send(query, cancellationToken);

        return Ok(groups);
    }
}