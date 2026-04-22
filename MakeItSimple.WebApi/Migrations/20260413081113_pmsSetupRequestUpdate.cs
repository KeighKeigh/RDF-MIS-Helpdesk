using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class pmsSetupRequestUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "pmss",
                newName: "charging");

            migrationBuilder.AddColumn<int>(
                name: "one_charging_id",
                table: "pmss",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "site_id",
                table: "pmss",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_pmss_one_charging_id",
                table: "pmss",
                column: "one_charging_id");

            migrationBuilder.CreateIndex(
                name: "ix_pmss_site_id",
                table: "pmss",
                column: "site_id");

            migrationBuilder.AddForeignKey(
                name: "fk_pmss_one_chargings_one_charging_id",
                table: "pmss",
                column: "one_charging_id",
                principalTable: "one_chargings",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_pmss_sites_site_id",
                table: "pmss",
                column: "site_id",
                principalTable: "sites",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pmss_one_chargings_one_charging_id",
                table: "pmss");

            migrationBuilder.DropForeignKey(
                name: "fk_pmss_sites_site_id",
                table: "pmss");

            migrationBuilder.DropIndex(
                name: "ix_pmss_one_charging_id",
                table: "pmss");

            migrationBuilder.DropIndex(
                name: "ix_pmss_site_id",
                table: "pmss");

            migrationBuilder.DropColumn(
                name: "one_charging_id",
                table: "pmss");

            migrationBuilder.DropColumn(
                name: "site_id",
                table: "pmss");

            migrationBuilder.RenameColumn(
                name: "charging",
                table: "pmss",
                newName: "name");
        }
    }
}
