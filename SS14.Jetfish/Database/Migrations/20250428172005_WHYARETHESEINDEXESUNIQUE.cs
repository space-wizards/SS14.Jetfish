using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SS14.Jetfish.Migrations
{
    /// <inheritdoc />
    public partial class WHYARETHESEINDEXESUNIQUE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FileUsage_CardId",
                table: "FileUsage");

            migrationBuilder.DropIndex(
                name: "IX_FileUsage_ProjectId",
                table: "FileUsage");

            migrationBuilder.CreateIndex(
                name: "IX_FileUsage_CardId",
                table: "FileUsage",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_FileUsage_ProjectId",
                table: "FileUsage",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FileUsage_CardId",
                table: "FileUsage");

            migrationBuilder.DropIndex(
                name: "IX_FileUsage_ProjectId",
                table: "FileUsage");

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
        }
    }
}
