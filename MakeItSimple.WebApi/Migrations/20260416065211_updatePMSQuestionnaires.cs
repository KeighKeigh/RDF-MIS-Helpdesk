using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class updatePMSQuestionnaires : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pms_answers_pms_section_question_section_question_id",
                table: "pms_answers");

            migrationBuilder.DropTable(
                name: "pms_section_question");

            migrationBuilder.DropTable(
                name: "pms_phase_two_question_category");

            migrationBuilder.DropTable(
                name: "pms_questions");

            migrationBuilder.DropTable(
                name: "pms_sections");

            migrationBuilder.CreateTable(
                name: "pms_section_questions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    section = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    headers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    has_header = table.Column<bool>(type: "bit", nullable: true),
                    questions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    has_check_box = table.Column<bool>(type: "bit", nullable: true),
                    has_remarks = table.Column<bool>(type: "bit", nullable: true),
                    has_asset_tag = table.Column<bool>(type: "bit", nullable: true),
                    has_paragraph = table.Column<bool>(type: "bit", nullable: true),
                    pms_type_id = table.Column<int>(type: "int", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    order_by = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pms_section_questions", x => x.id);
                    table.ForeignKey(
                        name: "fk_pms_section_questions_pms_type_pms_type_id",
                        column: x => x.pms_type_id,
                        principalTable: "pms_type",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_pms_section_questions_users_added_by_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_pms_section_questions_added_by",
                table: "pms_section_questions",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_pms_section_questions_pms_type_id",
                table: "pms_section_questions",
                column: "pms_type_id");

            migrationBuilder.AddForeignKey(
                name: "fk_pms_answers_pms_section_questions_section_question_id",
                table: "pms_answers",
                column: "section_question_id",
                principalTable: "pms_section_questions",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pms_answers_pms_section_questions_section_question_id",
                table: "pms_answers");

            migrationBuilder.DropTable(
                name: "pms_section_questions");

            migrationBuilder.CreateTable(
                name: "pms_phase_two_question_category",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    question_category = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pms_phase_two_question_category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pms_sections",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    pms_type_id = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    section = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pms_sections", x => x.id);
                    table.ForeignKey(
                        name: "fk_pms_sections_pms_type_pms_type_id",
                        column: x => x.pms_type_id,
                        principalTable: "pms_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_pms_sections_users_added_by_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pms_questions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    pms_type_id = table.Column<int>(type: "int", nullable: true),
                    has_asset_tag = table.Column<bool>(type: "bit", nullable: true),
                    has_check_box = table.Column<bool>(type: "bit", nullable: true),
                    has_remarks = table.Column<bool>(type: "bit", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    pms_phase_two_section_id = table.Column<int>(type: "int", nullable: true),
                    questions = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pms_questions", x => x.id);
                    table.ForeignKey(
                        name: "fk_pms_questions_pms_sections_pms_phase_two_section_id",
                        column: x => x.pms_phase_two_section_id,
                        principalTable: "pms_sections",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_pms_questions_pms_type_pms_type_id",
                        column: x => x.pms_type_id,
                        principalTable: "pms_type",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_pms_questions_users_added_by_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pms_section_question",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    category_id = table.Column<int>(type: "int", nullable: false),
                    question_id = table.Column<int>(type: "int", nullable: false),
                    section_id = table.Column<int>(type: "int", nullable: false),
                    order_by = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pms_section_question", x => x.id);
                    table.ForeignKey(
                        name: "fk_pms_section_question_pms_phase_two_question_category_category_id",
                        column: x => x.category_id,
                        principalTable: "pms_phase_two_question_category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_pms_section_question_pms_questions_question_id",
                        column: x => x.question_id,
                        principalTable: "pms_questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_pms_section_question_pms_sections_section_id",
                        column: x => x.section_id,
                        principalTable: "pms_sections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_pms_section_question_users_added_by_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_pms_questions_added_by",
                table: "pms_questions",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_pms_questions_pms_phase_two_section_id",
                table: "pms_questions",
                column: "pms_phase_two_section_id");

            migrationBuilder.CreateIndex(
                name: "ix_pms_questions_pms_type_id",
                table: "pms_questions",
                column: "pms_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_pms_section_question_added_by",
                table: "pms_section_question",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_pms_section_question_category_id",
                table: "pms_section_question",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_pms_section_question_question_id",
                table: "pms_section_question",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "ix_pms_section_question_section_id",
                table: "pms_section_question",
                column: "section_id");

            migrationBuilder.CreateIndex(
                name: "ix_pms_sections_added_by",
                table: "pms_sections",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_pms_sections_pms_type_id",
                table: "pms_sections",
                column: "pms_type_id");

            migrationBuilder.AddForeignKey(
                name: "fk_pms_answers_pms_section_question_section_question_id",
                table: "pms_answers",
                column: "section_question_id",
                principalTable: "pms_section_question",
                principalColumn: "id");
        }
    }
}
