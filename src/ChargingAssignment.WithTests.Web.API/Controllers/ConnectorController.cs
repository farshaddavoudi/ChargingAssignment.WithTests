using CharginAssignment.WithTests.Application.ConnectorUseCases.CreateConnector;
using CharginAssignment.WithTests.Application.ConnectorUseCases.DeleteConnector;
using CharginAssignment.WithTests.Application.ConnectorUseCases.GetConnectorMaxCurrent;
using CharginAssignment.WithTests.Application.ConnectorUseCases.UpdateConnectorMaxCurrent;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;

namespace CharginAssignment.WithTests.Web.API.Controllers;

public class ConnectorController(IMediator mediator) : BaseApiController
{
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status201Created)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, BusinessLogicDescription)]
    public async Task<IActionResult> Create(CreateConnectorCommand command, CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);

        return Created();
    }

    [HttpPatch]
    [SwaggerResponse(StatusCodes.Status204NoContent, UpdatedDescription)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, BusinessLogicDescription)]
    public async Task<IActionResult> UpdateConnectorMaxCurrent(UpdateConnectorMaxCurrentCommand command, CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{chargeStationId}/{connectorId}")]
    [SwaggerResponse(StatusCodes.Status204NoContent, DeletedDescription)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, BusinessLogicDescription)]
    public async Task<IActionResult> Delete(Guid chargeStationId, int connectorId, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteConnectorCommand(chargeStationId, connectorId), cancellationToken);

        return NoContent();
    }

    [HttpGet("{chargeStationId}/{connectorId}")]
    [SwaggerResponse(StatusCodes.Status200OK, FetchedDescription, typeof(int), JsonContentType)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetConnectorMaxCurrent(Guid chargeStationId, int connectorId, CancellationToken cancellationToken)
    {
        var maxCurrent = await mediator.Send(new GetConnectorMaxCurrentQuery(chargeStationId, connectorId), cancellationToken);

        return Ok(maxCurrent);
    }
}