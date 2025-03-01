using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MissionComplete.Migrations
{
    /// <inheritdoc />
    public partial class RemovePlaeyrForReal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChallengeCompletions_Challenges_ChallengeId",
                table: "ChallengeCompletions");

            migrationBuilder.DropForeignKey(
                name: "FK_ChallengeCompletions_Players_PlayerId",
                table: "ChallengeCompletions");

            migrationBuilder.DropForeignKey(
                name: "FK_ChallengeCompletions_Users_UserId",
                table: "ChallengeCompletions");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChallengeCompletions",
                table: "ChallengeCompletions");

            migrationBuilder.DropIndex(
                name: "IX_ChallengeCompletions_PlayerId",
                table: "ChallengeCompletions");

            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "ChallengeCompletions");

            migrationBuilder.RenameTable(
                name: "ChallengeCompletions",
                newName: "ChallengeCompletion");

            migrationBuilder.RenameIndex(
                name: "IX_ChallengeCompletions_UserId",
                table: "ChallengeCompletion",
                newName: "IX_ChallengeCompletion_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ChallengeCompletions_ChallengeId",
                table: "ChallengeCompletion",
                newName: "IX_ChallengeCompletion_ChallengeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChallengeCompletion",
                table: "ChallengeCompletion",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChallengeCompletion_Challenges_ChallengeId",
                table: "ChallengeCompletion",
                column: "ChallengeId",
                principalTable: "Challenges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChallengeCompletion_Users_UserId",
                table: "ChallengeCompletion",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChallengeCompletion_Challenges_ChallengeId",
                table: "ChallengeCompletion");

            migrationBuilder.DropForeignKey(
                name: "FK_ChallengeCompletion_Users_UserId",
                table: "ChallengeCompletion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChallengeCompletion",
                table: "ChallengeCompletion");

            migrationBuilder.RenameTable(
                name: "ChallengeCompletion",
                newName: "ChallengeCompletions");

            migrationBuilder.RenameIndex(
                name: "IX_ChallengeCompletion_UserId",
                table: "ChallengeCompletions",
                newName: "IX_ChallengeCompletions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ChallengeCompletion_ChallengeId",
                table: "ChallengeCompletions",
                newName: "IX_ChallengeCompletions_ChallengeId");

            migrationBuilder.AddColumn<int>(
                name: "PlayerId",
                table: "ChallengeCompletions",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChallengeCompletions",
                table: "ChallengeCompletions",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeCompletions_PlayerId",
                table: "ChallengeCompletions",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_TeamId",
                table: "Players",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChallengeCompletions_Challenges_ChallengeId",
                table: "ChallengeCompletions",
                column: "ChallengeId",
                principalTable: "Challenges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChallengeCompletions_Players_PlayerId",
                table: "ChallengeCompletions",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChallengeCompletions_Users_UserId",
                table: "ChallengeCompletions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
