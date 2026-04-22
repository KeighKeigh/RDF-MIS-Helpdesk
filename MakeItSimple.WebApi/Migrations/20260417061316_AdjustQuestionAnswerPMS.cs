using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AdjustQuestionAnswerPMS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "pms_section_questions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "has_textfield",
                table: "pms_section_questions",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "pms_section_questions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "updated_by",
                table: "pms_section_questions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "paragraph",
                table: "pms_answers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "textfield",
                table: "pms_answers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_pms_section_questions_updated_by",
                table: "pms_section_questions",
                column: "updated_by");

            migrationBuilder.AddForeignKey(
                name: "fk_pms_section_questions_users_updated_by_user_id",
                table: "pms_section_questions",
                column: "updated_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pms_section_questions_users_updated_by_user_id",
                table: "pms_section_questions");

            migrationBuilder.DropIndex(
                name: "ix_pms_section_questions_updated_by",
                table: "pms_section_questions");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "pms_section_questions");

            migrationBuilder.DropColumn(
                name: "has_textfield",
                table: "pms_section_questions");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "pms_section_questions");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "pms_section_questions");

            migrationBuilder.DropColumn(
                name: "paragraph",
                table: "pms_answers");

            migrationBuilder.DropColumn(
                name: "textfield",
                table: "pms_answers");
        }
    }
}
