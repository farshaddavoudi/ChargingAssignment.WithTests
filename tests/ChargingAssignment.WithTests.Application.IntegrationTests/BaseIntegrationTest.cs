using CharginAssignment.WithTests.Domain.Constants;
using CharginAssignment.WithTests.Infrastructure.Persistence.EFCore;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace CharginAssignment.WithTests.Application.IntegrationTests;

public class BaseIntegrationTest : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly IServiceScope _scope;
    protected readonly IMediator Mediator;
    protected readonly AppDbContext DbContext;
    protected IMemoryCache Cache;


    protected BaseIntegrationTest(CustomWebApplicationFactory factory)
    {
        _scope = factory.Services.CreateScope();

        Mediator = _scope.ServiceProvider.GetRequiredService<IMediator>();
        DbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Cache = _scope.ServiceProvider.GetRequiredService<IMemoryCache>();
    }

    public Task InitializeAsync()
    {
        ClearCache();

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
    }

    private void ClearCache()
    {
        Cache.Remove(CacheKeyConst.AllGroups);
    }
}