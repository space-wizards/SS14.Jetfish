using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SS14.Jetfish.Migrations
{
    /// <inheritdoc />
    public partial class UploadedFileIsConverting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsConverting",
                table: "UploadedFile",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConverting",
                table: "UploadedFile");
        }
    }
}
