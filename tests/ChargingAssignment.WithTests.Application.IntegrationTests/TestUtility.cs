using Bogus;
using CharginAssignment.WithTests.Application.ChargeStationUseCases.CreateChargeStation;
using CharginAssignment.WithTests.Application.ChargeStationUseCases.GetChargeStationById;
using CharginAssignment.WithTests.Application.GroupUseCases.CreateGroup;
using CharginAssignment.WithTests.Domain.Entities;
using CharginAssignment.WithTests.Infrastructure.Persistence.EFCore;
using MediatR;

namespace CharginAssignment.WithTests.Application.IntegrationTests;

internal static class TestUtility
{
    internal static string FakeName => new Faker().Name.FullName();

    internal static async Task<Guid> CreateGroup(IMediator mediator, string name, int capacity)
    {
        var createGroupCommand = new CreateGroupCommand(name, capacity);

        var newGroupId = await mediator.Send(createGroupCommand);

        return newGroupId;
    }

    internal static async Task RemoveAllTableRecords<TEntity>(AppDbContext dbContext) where TEntity : class
    {
        var dbSet = dbContext.Set<TEntity>();

        var allTableData = dbSet.ToList();

        dbSet.RemoveRange(allTableData);

        await dbContext.SaveChangesAsync();
    }

    public static async Task<(Guid groupId, Guid ChargeStationId)> CreateGroupWithChargeStation(IMediator mediator, int groupCapacity, string chargeStationName, params (int connectorId, int connectorMaxCurrewnt)[] connectors)
    {
        var newGroupId = await CreateGroup(mediator, TestUtility.FakeName, groupCapacity);

        var newChargeStationId = await CreateChargeStationInSpecificGroup(mediator, newGroupId, chargeStationName, connectors);

        return (newGroupId, newChargeStationId);
    }

    public static async Task<Guid> CreateChargeStation(IMediator mediator, int groupCapacity, string chargeStationName, params (int connectorId, int connectorMaxCurrewnt)[] connectors)
    {
        var result = await CreateGroupWithChargeStation(mediator, groupCapacity, chargeStationName, connectors);

        return result.ChargeStationId;
    }

    public static async Task<Guid> CreateChargeStationInSpecificGroup(IMediator mediator, Guid groupId, string chargeStationName, params (int connectorId, int connectorMaxCurrewnt)[] connectors)
    {
        List<ConnectorDto> connectorDtos = connectors.Select(c => new ConnectorDto(c.connectorId, c.connectorMaxCurrewnt)).ToList();

        var command = new CreateChargeStationCommand(groupId, chargeStationName, connectorDtos);

        var newChargeStationId = await mediator.Send(command);

        return newChargeStationId;
    }

    public static async Task<GroupEntity?> FetchGroupById(AppDbContext dbContext, Guid groupId)
    {
        return await dbContext.Set<GroupEntity>().FindAsync(groupId);
    }

    public static async Task<ChargeStationEntity?> FetchChargeStationById(AppDbContext dbContext, Guid chargeStationId)
    {
        return await dbContext.Set<ChargeStationEntity>().FindAsync(chargeStationId);
    }
}