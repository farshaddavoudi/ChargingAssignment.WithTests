using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CharginAssignment.WithTests.Infrastructure.Persistence.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class ModifyImplicitFkColumnNameInConnectorsTableToChargeStationId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Connectors_ChargeStations_ChargeStationEntityId",
                table: "Connectors");

            migrationBuilder.RenameColumn(
                name: "ChargeStationEntityId",
                table: "Connectors",
                newName: "ChargeStationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Connectors_ChargeStations_ChargeStationId",
                table: "Connectors",
                column: "ChargeStationId",
                principalTable: "ChargeStations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Connectors_ChargeStations_ChargeStationId",
                table: "Connectors");

            migrationBuilder.RenameColumn(
                name: "ChargeStationId",
                table: "Connectors",
                newName: "ChargeStationEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Connectors_ChargeStations_ChargeStationEntityId",
                table: "Connectors",
                column: "ChargeStationEntityId",
                principalTable: "ChargeStations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
