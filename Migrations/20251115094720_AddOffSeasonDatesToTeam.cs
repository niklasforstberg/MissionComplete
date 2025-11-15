using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MissionComplete.Migrations
{
    /// <inheritdoc />
    public partial class AddOffSeasonDatesToTeam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OffSeasonEndDate",
                table: "Teams",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OffSeasonStartDate",
                table: "Teams",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OffSeasonEndDate",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "OffSeasonStartDate",
                table: "Teams");
        }
    }
}
