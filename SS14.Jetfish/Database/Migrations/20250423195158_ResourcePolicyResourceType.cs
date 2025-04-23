using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SS14.Jetfish.Migrations
{
    /// <inheritdoc />
    public partial class ResourcePolicyResourceType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "ResourceType",
                table: "User_ResourcePolicies",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "ResourceType",
                table: "Role_Policies",
                type: "smallint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResourceType",
                table: "User_ResourcePolicies");

            migrationBuilder.DropColumn(
                name: "ResourceType",
                table: "Role_Policies");
        }
    }
}
