using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class ChecklistManagementPMS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "order_by",
                table: "pms_section_questions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "checklist_management_id",
                table: "pms_section_questions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "pms_checklist_managements",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pms_checklist_managements", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_pms_section_questions_checklist_management_id",
                table: "pms_section_questions",
                column: "checklist_management_id");

            migrationBuilder.AddForeignKey(
                name: "fk_pms_section_questions_pms_checklist_managements_checklist_management_id",
                table: "pms_section_questions",
                column: "checklist_management_id",
                principalTable: "pms_checklist_managements",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pms_section_questions_pms_checklist_managements_checklist_management_id",
                table: "pms_section_questions");

            migrationBuilder.DropTable(
                name: "pms_checklist_managements");

            migrationBuilder.DropIndex(
                name: "ix_pms_section_questions_checklist_management_id",
                table: "pms_section_questions");

            migrationBuilder.DropColumn(
                name: "checklist_management_id",
                table: "pms_section_questions");

            migrationBuilder.AlterColumn<int>(
                name: "order_by",
                table: "pms_section_questions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
