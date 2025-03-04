using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MissionComplete.Migrations
{
    /// <inheritdoc />
    public partial class AddChallangeCompletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChallengeCompletions",
                table: "ChallengeCompletions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChallengeCompletions_Challenges_ChallengeId",
                table: "ChallengeCompletions",
                column: "ChallengeId",
                principalTable: "Challenges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChallengeCompletions_Users_UserId",
                table: "ChallengeCompletions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChallengeCompletions_Challenges_ChallengeId",
                table: "ChallengeCompletions");

            migrationBuilder.DropForeignKey(
                name: "FK_ChallengeCompletions_Users_UserId",
                table: "ChallengeCompletions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChallengeCompletions",
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
    }
}
