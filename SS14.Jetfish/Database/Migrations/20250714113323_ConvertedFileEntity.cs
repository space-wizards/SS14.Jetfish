using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SS14.Jetfish.Migrations
{
    /// <inheritdoc />
    public partial class ConvertedFileEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConvertedFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    UploadedFileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Label = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                    RelativePath = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                    MimeType = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    Etag = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConvertedFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConvertedFile_UploadedFile_UploadedFileId",
                        column: x => x.UploadedFileId,
                        principalTable: "UploadedFile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConvertedFile_UploadedFileId",
                table: "ConvertedFile",
                column: "UploadedFileId");

            migrationBuilder.Sql(@"
                CREATE TRIGGER ""BeforeUpdateConvertedFileTrigger""
                    BEFORE UPDATE ON ""ConvertedFile""
                    FOR EACH ROW
                EXECUTE FUNCTION fn_incremet_version();");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"DROP TRIGGER ""BeforeUpdateConvertedFileTrigger"" ON ""ConvertedFile"";");
            migrationBuilder.DropTable(
                name: "ConvertedFile");
        }
    }
}
