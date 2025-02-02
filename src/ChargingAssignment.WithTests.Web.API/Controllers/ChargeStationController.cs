using CharginAssignment.WithTests.Application.ChargeStationUseCases.CreateChargeStation;
using CharginAssignment.WithTests.Application.ChargeStationUseCases.DeleteChargeStation;
using CharginAssignment.WithTests.Application.ChargeStationUseCases.GetAllChargeStationsByGroup;
using CharginAssignment.WithTests.Application.ChargeStationUseCases.GetChargeStationById;
using CharginAssignment.WithTests.Application.ChargeStationUseCases.UpdateChargeStationName;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;

namespace CharginAssignment.WithTests.Web.API.Controllers;

public class ChargeStationController(IMediator mediator) : BaseApiController
{
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status201Created, CreatedDescription, typeof(Guid), JsonContentType)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, BusinessLogicDescription)]
    public async Task<IActionResult> Create(CreateChargeStationCommand command, CancellationToken cancellationToken)
    {
        var newChargeStationId = await mediator.Send(command, cancellationToken);

        return Created(newChargeStationId);
    }

    [HttpPut]
    [SwaggerResponse(StatusCodes.Status204NoContent, UpdatedDescription)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateName(UpdateChargeStationNameCommand command, CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{chargeStationId}")]
    [SwaggerResponse(StatusCodes.Status204NoContent, UpdatedDescription)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid chargeStationId, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteChargeStationCommand(chargeStationId), cancellationToken);

        return NoContent();
    }

    [HttpGet("{id}")]
    [SwaggerResponse(StatusCodes.Status200OK, FetchedDescription, typeof(ChargeStationDto), JsonContentType)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetChargeStationById(Guid id, CancellationToken cancellationToken)
    {
        var chargeStation = await mediator.Send(new GetChargeStationByIdQuery(id), cancellationToken);

        return Ok(chargeStation);
    }

    [HttpGet("{groupId}")]
    [SwaggerResponse(StatusCodes.Status200OK, FetchedDescription, typeof(List<ChargeStationDto>), JsonContentType)]
    public async Task<IActionResult> GetChargeStationsByGroupId(Guid groupId, CancellationToken cancellationToken)
    {
        var chargeStations = await mediator.Send(new GetChargeStationsByGroupQuery(groupId), cancellationToken);

        return Ok(chargeStations);
    }
}