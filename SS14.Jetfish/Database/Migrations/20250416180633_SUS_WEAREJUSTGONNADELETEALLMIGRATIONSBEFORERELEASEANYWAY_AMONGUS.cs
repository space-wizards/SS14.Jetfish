using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SS14.Jetfish.Migrations
{
    /// <inheritdoc />
    public partial class SUS_WEAREJUSTGONNADELETEALLMIGRATIONSBEFORERELEASEANYWAY_AMONGUS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectUploadedFile");

            migrationBuilder.CreateTable(
                name: "Card",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Card", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Card_User_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lane",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lane", x => new { x.ProjectId, x.Id });
                    table.ForeignKey(
                        name: "FK_Lane_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileUsage",
                columns: table => new
                {
                    CardId = table.Column<Guid>(type: "uuid", nullable: false),
                    UploadedFileId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileUsage", x => new { x.CardId, x.UploadedFileId });
                    table.ForeignKey(
                        name: "FK_FileUsage_Card_CardId",
                        column: x => x.CardId,
                        principalTable: "Card",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileUsage_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileUsage_UploadedFile_UploadedFileId",
                        column: x => x.UploadedFileId,
                        principalTable: "UploadedFile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Card_AuthorId",
                table: "Card",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_FileUsage_CardId",
                table: "FileUsage",
                column: "CardId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileUsage_ProjectId",
                table: "FileUsage",
                column: "ProjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileUsage_UploadedFileId",
                table: "FileUsage",
                column: "UploadedFileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileUsage");

            migrationBuilder.DropTable(
                name: "Lane");

            migrationBuilder.DropTable(
                name: "Card");

            migrationBuilder.CreateTable(
                name: "ProjectUploadedFile",
                columns: table => new
                {
                    UploadedFileId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsedInProjectsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectUploadedFile", x => new { x.UploadedFileId, x.UsedInProjectsId });
                    table.ForeignKey(
                        name: "FK_ProjectUploadedFile_Project_UsedInProjectsId",
                        column: x => x.UsedInProjectsId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectUploadedFile_UploadedFile_UploadedFileId",
                        column: x => x.UploadedFileId,
                        principalTable: "UploadedFile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUploadedFile_UsedInProjectsId",
                table: "ProjectUploadedFile",
                column: "UsedInProjectsId");
        }
    }
}
