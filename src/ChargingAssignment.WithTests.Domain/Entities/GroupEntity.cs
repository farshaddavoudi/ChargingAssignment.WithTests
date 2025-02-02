namespace CharginAssignment.WithTests.Domain.Entities;

public class GroupEntity : IEntity
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public int Capacity { get; set; }

    // Nav
    public ICollection<ChargeStationEntity> ChargeStations { get; set; } = new List<ChargeStationEntity>();
}