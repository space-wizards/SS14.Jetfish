using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SS14.Jetfish.Migrations
{
    /// <inheritdoc />
    public partial class LittleCreatureHaveYouEverHeardOfGayPeople : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Background",
                table: "Project",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "BackgroundSpecifier",
                table: "Project",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Background",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "BackgroundSpecifier",
                table: "Project");
        }
    }
}
