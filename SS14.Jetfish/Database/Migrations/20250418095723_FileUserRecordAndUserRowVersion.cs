using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SS14.Jetfish.Migrations
{
    /// <inheritdoc />
    public partial class FileUserRecordAndUserRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileUsage_Card_CardId",
                table: "FileUsage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FileUsage",
                table: "FileUsage");

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "UploadedFile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "CardId",
                table: "FileUsage",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "FileUsage",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileUsage",
                table: "FileUsage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FileUsage_Card_CardId",
                table: "FileUsage",
                column: "CardId",
                principalTable: "Card",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileUsage_Card_CardId",
                table: "FileUsage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FileUsage",
                table: "FileUsage");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "UploadedFile");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "FileUsage");

            migrationBuilder.AlterColumn<Guid>(
                name: "CardId",
                table: "FileUsage",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileUsage",
                table: "FileUsage",
                columns: new[] { "CardId", "UploadedFileId" });

            migrationBuilder.AddForeignKey(
                name: "FK_FileUsage_Card_CardId",
                table: "FileUsage",
                column: "CardId",
                principalTable: "Card",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
