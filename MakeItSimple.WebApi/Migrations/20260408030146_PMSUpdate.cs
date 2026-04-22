using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class PMSUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pms_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pms_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pms_type", x => x.id);
                    table.ForeignKey(
                        name: "fk_pms_type_users_added_by_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pms_sections",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    section = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pms_type_id = table.Column<int>(type: "int", nullable: false),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true)
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
                name: "pmss",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    requestor_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    schedule_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    time_in = table.Column<DateTime>(type: "datetime2", nullable: true),
                    time_out = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_completed = table.Column<bool>(type: "bit", nullable: true),
                    is_canceled = table.Column<bool>(type: "bit", nullable: true),
                    pms_type_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pmss", x => x.id);
                    table.ForeignKey(
                        name: "fk_pmss_pms_type_pms_type_id",
                        column: x => x.pms_type_id,
                        principalTable: "pms_type",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_pmss_users_added_by_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_pmss_users_requestor_id",
                        column: x => x.requestor_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "pms_questions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    questions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    has_check_box = table.Column<bool>(type: "bit", nullable: true),
                    has_remarks = table.Column<bool>(type: "bit", nullable: true),
                    has_asset_tag = table.Column<bool>(type: "bit", nullable: true),
                    pms_type_id = table.Column<int>(type: "int", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    pms_phase_two_section_id = table.Column<int>(type: "int", nullable: true)
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
                    section_id = table.Column<int>(type: "int", nullable: false),
                    question_id = table.Column<int>(type: "int", nullable: false),
                    order_by = table.Column<int>(type: "int", nullable: false),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pms_section_question", x => x.id);
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

            migrationBuilder.CreateTable(
                name: "pms_answers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pms_id = table.Column<int>(type: "int", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    section_question_id = table.Column<int>(type: "int", nullable: true),
                    status_answer = table.Column<bool>(type: "bit", nullable: true),
                    remarks_answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    asset_tag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pms_answers", x => x.id);
                    table.ForeignKey(
                        name: "fk_pms_answers_pms_section_question_section_question_id",
                        column: x => x.section_question_id,
                        principalTable: "pms_section_question",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_pms_answers_pmss_pms_id",
                        column: x => x.pms_id,
                        principalTable: "pmss",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_pms_answers_users_added_by_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_pms_answers_added_by",
                table: "pms_answers",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_pms_answers_pms_id",
                table: "pms_answers",
                column: "pms_id");

            migrationBuilder.CreateIndex(
                name: "ix_pms_answers_section_question_id",
                table: "pms_answers",
                column: "section_question_id");

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

            migrationBuilder.CreateIndex(
                name: "ix_pms_type_added_by",
                table: "pms_type",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_pmss_added_by",
                table: "pmss",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_pmss_pms_type_id",
                table: "pmss",
                column: "pms_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_pmss_requestor_id",
                table: "pmss",
                column: "requestor_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pms_answers");

            migrationBuilder.DropTable(
                name: "pms_section_question");

            migrationBuilder.DropTable(
                name: "pmss");

            migrationBuilder.DropTable(
                name: "pms_questions");

            migrationBuilder.DropTable(
                name: "pms_sections");

            migrationBuilder.DropTable(
                name: "pms_type");
        }
    }
}
