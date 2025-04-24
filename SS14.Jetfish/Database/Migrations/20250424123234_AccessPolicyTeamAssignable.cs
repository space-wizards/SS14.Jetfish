using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SS14.Jetfish.Migrations
{
    /// <inheritdoc />
    public partial class AccessPolicyTeamAssignable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TeamAssignable",
                table: "AccessPolicies",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeamAssignable",
                table: "AccessPolicies");
        }
    }
}
