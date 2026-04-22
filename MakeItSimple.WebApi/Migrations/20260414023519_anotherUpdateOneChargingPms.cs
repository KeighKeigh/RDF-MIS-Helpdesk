using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class anotherUpdateOneChargingPms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pmss_one_chargings_one_charging_id",
                table: "pmss");

            migrationBuilder.DropIndex(
                name: "ix_pmss_one_charging_id",
                table: "pmss");

            migrationBuilder.DropColumn(
                name: "charging",
                table: "pmss");

            migrationBuilder.DropColumn(
                name: "one_charging_id",
                table: "pmss");

            migrationBuilder.AddColumn<string>(
                name: "charging_code",
                table: "pmss",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_pmss_charging_code",
                table: "pmss",
                column: "charging_code");

            migrationBuilder.AddForeignKey(
                name: "fk_pmss_one_chargings_charging_code",
                table: "pmss",
                column: "charging_code",
                principalTable: "one_chargings",
                principalColumn: "code",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pmss_one_chargings_charging_code",
                table: "pmss");

            migrationBuilder.DropIndex(
                name: "ix_pmss_charging_code",
                table: "pmss");

            migrationBuilder.DropColumn(
                name: "charging_code",
                table: "pmss");

            migrationBuilder.AddColumn<int>(
                name: "charging",
                table: "pmss",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "one_charging_id",
                table: "pmss",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_pmss_one_charging_id",
                table: "pmss",
                column: "one_charging_id");

            migrationBuilder.AddForeignKey(
                name: "fk_pmss_one_chargings_one_charging_id",
                table: "pmss",
                column: "one_charging_id",
                principalTable: "one_chargings",
                principalColumn: "id");
        }
    }
}
