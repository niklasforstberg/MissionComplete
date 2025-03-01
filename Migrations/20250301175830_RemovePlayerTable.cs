using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MissionComplete.Migrations
{
    /// <inheritdoc />
    public partial class RemovePlayerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PlayerId",
                table: "ChallengeCompletions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ChallengeCompletions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeCompletions_UserId",
                table: "ChallengeCompletions",
                column: "UserId");

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
                name: "FK_ChallengeCompletions_Users_UserId",
                table: "ChallengeCompletions");

            migrationBuilder.DropIndex(
                name: "IX_ChallengeCompletions_UserId",
                table: "ChallengeCompletions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ChallengeCompletions");

            migrationBuilder.AlterColumn<int>(
                name: "PlayerId",
                table: "ChallengeCompletions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
