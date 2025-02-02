using FluentAssertions;
using CharginAssignment.WithTests.Application.GroupUseCases.CreateGroup;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace CharginAssignment.WithTests.Application.IntegrationTests;

public class GroupControllerTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _customWebApplicationFactory;
    private readonly HttpClient _apiClient;

    public GroupControllerTests(CustomWebApplicationFactory customWebApplicationFactory)
    {
        _customWebApplicationFactory = customWebApplicationFactory;
        _apiClient = _customWebApplicationFactory.CreateClient();
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task CreateGroup_WhenRequestIsValid_Returns201CreatedCode()
    {
        CreateGroupCommand command = new("GroupTest", 10);

        var content = new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");

        var response = await _apiClient.PostAsync("api/Group/Create", content);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task UpdateGroup_WhenGivenCapacityLessThanRelatedConnectorsMaxCurrentSum_Throws422Exception()
    {
    }

    public async Task DisposeAsync()
    {
        _apiClient.Dispose();
        await _customWebApplicationFactory.DisposeAsync();
    }
}