using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class newSitesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "business_unit",
                table: "sites");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "sites",
                newName: "site");

            migrationBuilder.CreateTable(
                name: "sites_pivot",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    business_unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sites_pivot", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sites_pivot");

            migrationBuilder.RenameColumn(
                name: "site",
                table: "sites",
                newName: "name");

            migrationBuilder.AddColumn<string>(
                name: "business_unit",
                table: "sites",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
