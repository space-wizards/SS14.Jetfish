using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SS14.Jetfish.Migrations
{
    /// <inheritdoc />
    public partial class AdminArea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short[]>(
                name: "AccessAreas",
                table: "AccessPolicies",
                type: "smallint[]",
                nullable: false,
                oldClrType: typeof(int[]),
                oldType: "integer[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int[]>(
                name: "AccessAreas",
                table: "AccessPolicies",
                type: "integer[]",
                nullable: false,
                oldClrType: typeof(short[]),
                oldType: "smallint[]");
        }
    }
}
