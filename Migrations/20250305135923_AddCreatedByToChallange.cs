using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MissionComplete.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByToChallange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Challenges",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_CreatedById",
                table: "Challenges",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_Users_CreatedById",
                table: "Challenges",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_Users_CreatedById",
                table: "Challenges");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_CreatedById",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Challenges");
        }
    }
}
