using CharginAssignment.WithTests.Domain.Entities;

namespace CharginAssignment.WithTests.Application.Common.Contracts.Repositories;

public interface IChargeStationRepository
{
    Task AddChargeStation(ChargeStationEntity chargeStation, CancellationToken cancellationToken);

    void UpdateChargeStation(ChargeStationEntity chargeStation);

    void DeleteChargeStation(ChargeStationEntity chargeStation);

    Task<List<ChargeStationEntity>> GetAllChargeStations(CancellationToken cancellationToken);

    Task<List<ChargeStationEntity>> GetChargeStationsByGroupId(Guid groupId, CancellationToken cancellationToken);

    Task<ChargeStationEntity?> GetChargeStationById(Guid id, CancellationToken cancellationToken);

    Task<ChargeStationEntity?> GetChargeStationWithGroupById(Guid id, CancellationToken cancellationToken);

    Task<bool> DoesConnectorExist(Guid chargeStationId, int connectorId, CancellationToken cancellationToken);

    Task<int> GetConnectorMaxCurrent(Guid chargeStationId, int connectorId, CancellationToken cancellationToken);
}