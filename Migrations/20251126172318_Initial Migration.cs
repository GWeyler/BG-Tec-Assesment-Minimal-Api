using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BG_Tec_Assesment_Minimal_Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Travellers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Forename = table.Column<string>(type: "varchar(255)", nullable: false),
                    Surname = table.Column<string>(type: "varchar(255)", nullable: false),
                    Dob = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNumber = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false),
                    DocumentNumberSHA = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Travellers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlightTraveller",
                columns: table => new
                {
                    FlightsId = table.Column<int>(type: "int", nullable: false),
                    TravellersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightTraveller", x => new { x.FlightsId, x.TravellersId });
                    table.ForeignKey(
                        name: "FK_FlightTraveller_Flights_FlightsId",
                        column: x => x.FlightsId,
                        principalTable: "Flights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlightTraveller_Travellers_TravellersId",
                        column: x => x.TravellersId,
                        principalTable: "Travellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Flights",
                column: "Id",
                values: new object[]
                {
                    1552,
                    1553,
                    1554
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlightTraveller_TravellersId",
                table: "FlightTraveller",
                column: "TravellersId");

            migrationBuilder.CreateIndex(
                name: "IX_Travellers_DocumentNumberSHA",
                table: "Travellers",
                column: "DocumentNumberSHA",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightTraveller");

            migrationBuilder.DropTable(
                name: "Flights");

            migrationBuilder.DropTable(
                name: "Travellers");
        }
    }
}
