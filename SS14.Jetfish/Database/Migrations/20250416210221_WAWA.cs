using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SS14.Jetfish.Migrations
{
    /// <inheritdoc />
    public partial class WAWA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Lane",
                table: "Lane");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Lane",
                newName: "Version");

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "User",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "TeamMember",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Team",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Role",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Project",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LaneId",
                table: "Lane",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Card",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "AccessPolicies",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lane",
                table: "Lane",
                columns: new[] { "ProjectId", "LaneId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Lane",
                table: "Lane");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "TeamMember");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "LaneId",
                table: "Lane");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Card");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "AccessPolicies");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "Lane",
                newName: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lane",
                table: "Lane",
                columns: new[] { "ProjectId", "Id" });
        }
    }
}
