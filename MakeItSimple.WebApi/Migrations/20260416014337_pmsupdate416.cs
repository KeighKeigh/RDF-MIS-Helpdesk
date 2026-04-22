using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class pmsupdate416 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "category_id",
                table: "pms_section_question",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "pms_phase_two_question_category",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    question_category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pms_phase_two_question_category", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_pms_section_question_category_id",
                table: "pms_section_question",
                column: "category_id");

            migrationBuilder.AddForeignKey(
                name: "fk_pms_section_question_pms_phase_two_question_category_category_id",
                table: "pms_section_question",
                column: "category_id",
                principalTable: "pms_phase_two_question_category",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pms_section_question_pms_phase_two_question_category_category_id",
                table: "pms_section_question");

            migrationBuilder.DropTable(
                name: "pms_phase_two_question_category");

            migrationBuilder.DropIndex(
                name: "ix_pms_section_question_category_id",
                table: "pms_section_question");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "pms_section_question");
        }
    }
}
