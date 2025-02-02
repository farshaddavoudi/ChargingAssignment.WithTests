namespace CharginAssignment.WithTests.Application.ChargeStationUseCases.GetChargeStationById;

public record ChargeStationDto(Guid Id, Guid GroupId, string? Name, List<ConnectorDto> Connectors);