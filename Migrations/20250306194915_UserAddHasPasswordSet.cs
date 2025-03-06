using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MissionComplete.Migrations
{
    /// <inheritdoc />
    public partial class UserAddHasPasswordSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasPasswordSet",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasPasswordSet",
                table: "Users");
        }
    }
}
