namespace CharginAssignment.WithTests.Domain.Entities;

public class ChargeStationEntity : IEntity
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public Guid GroupId { get; set; }

    // Owned
    public ICollection<ConnectorEntity> Connectors { get; set; } = new List<ConnectorEntity>();

    // Nav
    public GroupEntity? Group { get; set; }
}