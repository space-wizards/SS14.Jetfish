using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SS14.Jetfish.Migrations
{
    /// <inheritdoc />
    public partial class A : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Role_Policies_AccessPolicy_AccessPolicyId",
                table: "Role_Policies");

            migrationBuilder.DropForeignKey(
                name: "FK_User_ResourcePolicies_AccessPolicy_AccessPolicyId",
                table: "User_ResourcePolicies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccessPolicy",
                table: "AccessPolicy");

            migrationBuilder.RenameTable(
                name: "AccessPolicy",
                newName: "AccessPolicies");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccessPolicies",
                table: "AccessPolicies",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Role_Policies_AccessPolicies_AccessPolicyId",
                table: "Role_Policies",
                column: "AccessPolicyId",
                principalTable: "AccessPolicies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_ResourcePolicies_AccessPolicies_AccessPolicyId",
                table: "User_ResourcePolicies",
                column: "AccessPolicyId",
                principalTable: "AccessPolicies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Role_Policies_AccessPolicies_AccessPolicyId",
                table: "Role_Policies");

            migrationBuilder.DropForeignKey(
                name: "FK_User_ResourcePolicies_AccessPolicies_AccessPolicyId",
                table: "User_ResourcePolicies");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccessPolicies",
                table: "AccessPolicies");

            migrationBuilder.RenameTable(
                name: "AccessPolicies",
                newName: "AccessPolicy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccessPolicy",
                table: "AccessPolicy",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Role_Policies_AccessPolicy_AccessPolicyId",
                table: "Role_Policies",
                column: "AccessPolicyId",
                principalTable: "AccessPolicy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_ResourcePolicies_AccessPolicy_AccessPolicyId",
                table: "User_ResourcePolicies",
                column: "AccessPolicyId",
                principalTable: "AccessPolicy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
