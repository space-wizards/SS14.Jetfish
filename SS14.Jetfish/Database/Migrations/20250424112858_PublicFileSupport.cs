using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SS14.Jetfish.Migrations
{
    /// <inheritdoc />
    public partial class PublicFileSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileUsage_Project_ProjectId",
                table: "FileUsage");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProjectId",
                table: "FileUsage",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<bool>(
                name: "Public",
                table: "FileUsage",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_FileUsage_Public",
                table: "FileUsage",
                column: "Public");

            migrationBuilder.AddForeignKey(
                name: "FK_FileUsage_Project_ProjectId",
                table: "FileUsage",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileUsage_Project_ProjectId",
                table: "FileUsage");

            migrationBuilder.DropIndex(
                name: "IX_FileUsage_Public",
                table: "FileUsage");

            migrationBuilder.DropColumn(
                name: "Public",
                table: "FileUsage");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProjectId",
                table: "FileUsage",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FileUsage_Project_ProjectId",
                table: "FileUsage",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
