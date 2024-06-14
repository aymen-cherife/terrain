using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace terrain.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationDateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the old column
            migrationBuilder.DropColumn(
                name: "DateEtHeureDebut",
                table: "Reservations");

            // Create the new ReservationDates table
            migrationBuilder.CreateTable(
                name: "ReservationDates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    HeureDebut = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    Status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TerrainId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservationDates_Terrains_TerrainId",
                        column: x => x.TerrainId,
                        principalTable: "Terrains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Add the new foreign key column to Reservations
            migrationBuilder.AddColumn<int>(
                name: "ReservationDateId",
                table: "Reservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Update the data to ensure consistency (example query, adjust as needed)
            migrationBuilder.Sql(
                "UPDATE Reservations SET ReservationDateId = (SELECT MIN(Id) FROM ReservationDates)");

            // Update Manager data
            migrationBuilder.UpdateData(
                table: "Managers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$LsVeyXvzOKSH8D/7KyuewudYu2ZLlICRpFr/CAc.uU2vxJjlshfLW");

            // Create indices for the new columns
            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ReservationDateId",
                table: "Reservations",
                column: "ReservationDateId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationDates_TerrainId",
                table: "ReservationDates",
                column: "TerrainId");

            // Add the foreign key constraint
            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_ReservationDates_ReservationDateId",
                table: "Reservations",
                column: "ReservationDateId",
                principalTable: "ReservationDates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the foreign key constraint
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_ReservationDates_ReservationDateId",
                table: "Reservations");

            // Drop the ReservationDates table
            migrationBuilder.DropTable(
                name: "ReservationDates");

            // Remove the new foreign key column
            migrationBuilder.DropIndex(
                name: "IX_Reservations_ReservationDateId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ReservationDateId",
                table: "Reservations");

            // Add back the old column
            migrationBuilder.AddColumn<DateTime>(
                name: "DateEtHeureDebut",
                table: "Reservations",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            // Update Manager data
            migrationBuilder.UpdateData(
                table: "Managers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$GruSm4eJ8rIMvLMUqggz0.qeFstOjBCwlTEYLBfMg0JRRF1fBAQ9G");
        }
    }
}
