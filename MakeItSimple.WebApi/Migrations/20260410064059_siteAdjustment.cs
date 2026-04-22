using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class siteAdjustment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "business_unit",
                table: "sites_pivot");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "sites_pivot",
                newName: "business_unit_name");

            migrationBuilder.AddColumn<int>(
                name: "business_unit_id",
                table: "sites_pivot",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "site_id",
                table: "sites_pivot",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_sites_pivot_site_id",
                table: "sites_pivot",
                column: "site_id");

            migrationBuilder.AddForeignKey(
                name: "fk_sites_pivot_sites_site_id",
                table: "sites_pivot",
                column: "site_id",
                principalTable: "sites",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sites_pivot_sites_site_id",
                table: "sites_pivot");

            migrationBuilder.DropIndex(
                name: "ix_sites_pivot_site_id",
                table: "sites_pivot");

            migrationBuilder.DropColumn(
                name: "business_unit_id",
                table: "sites_pivot");

            migrationBuilder.DropColumn(
                name: "site_id",
                table: "sites_pivot");

            migrationBuilder.RenameColumn(
                name: "business_unit_name",
                table: "sites_pivot",
                newName: "name");

            migrationBuilder.AddColumn<string>(
                name: "business_unit",
                table: "sites_pivot",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
