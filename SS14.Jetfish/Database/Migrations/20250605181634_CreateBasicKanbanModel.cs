using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SS14.Jetfish.Migrations
{
    /// <inheritdoc />
    public partial class CreateBasicKanbanModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Order",
                table: "List",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "List",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Card",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: DateTime.UtcNow);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Card",
                type: "Text",
                maxLength: 50000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ListId",
                table: "Card",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "Order",
                table: "Card",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "Card",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Card",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Card",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.CreateTable(
                name: "CardComment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    CardId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "Text", maxLength: 50000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardComment_Card_CardId",
                        column: x => x.CardId,
                        principalTable: "Card",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardComment_User_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Card_ProjectId_ListId",
                table: "Card",
                columns: new[] { "ProjectId", "ListId" });

            migrationBuilder.CreateIndex(
                name: "IX_CardComment_AuthorId",
                table: "CardComment",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_CardComment_CardId",
                table: "CardComment",
                column: "CardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Card_List_ProjectId_ListId",
                table: "Card",
                columns: new[] { "ProjectId", "ListId" },
                principalTable: "List",
                principalColumns: new[] { "ProjectId", "ListId" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Card_List_ProjectId_ListId",
                table: "Card");

            migrationBuilder.DropTable(
                name: "CardComment");

            migrationBuilder.DropIndex(
                name: "IX_Card_ProjectId_ListId",
                table: "Card");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "List");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "List");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Card");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Card");

            migrationBuilder.DropColumn(
                name: "ListId",
                table: "Card");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Card");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Card");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Card");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Card");
        }
    }
}
