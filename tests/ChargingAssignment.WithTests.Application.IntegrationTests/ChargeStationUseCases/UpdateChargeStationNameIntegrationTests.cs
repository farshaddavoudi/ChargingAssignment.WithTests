using Bogus;
using FluentAssertions;
using CharginAssignment.WithTests.Application.ChargeStationUseCases.UpdateChargeStationName;

namespace CharginAssignment.WithTests.Application.IntegrationTests.ChargeStationUseCases;

public class UpdateChargeStationNameIntegrationTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async void UpdateChargeStationName_WhenGivenNewName_ChangesTheChargeStationNameInDatabase()
    {
        // Arrange
        string oldName = "ChargeStationOld";
        string newName = "ChangeStationNew";

        var newGeneratedChargeStationId = await TestUtility.CreateChargeStation(
            Mediator, new Faker().Random.Int(1), oldName, (1, 1));

        // Act
        await Mediator.Send(new UpdateChargeStationNameCommand(newGeneratedChargeStationId, newName));
        var fetchedChargeStation = await TestUtility.FetchChargeStationById(DbContext, newGeneratedChargeStationId);

        // Assert
        newGeneratedChargeStationId.Should().NotBeEmpty();
        fetchedChargeStation.Should().NotBeNull();
        fetchedChargeStation!.Name.Should().Be(newName);
    }
}