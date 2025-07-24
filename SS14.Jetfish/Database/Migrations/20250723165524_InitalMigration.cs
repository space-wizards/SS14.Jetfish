using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SS14.Jetfish.Migrations
{
    /// <inheritdoc />
    public partial class InitalMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessPolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Version = table.Column<int>(type: "integer", rowVersion: true, nullable: false, defaultValue: 0),
                    Permissions = table.Column<short[]>(type: "smallint[]", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TeamAssignable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationStore",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationStore", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<int>(type: "integer", rowVersion: true, nullable: false, defaultValue: 0),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    BackgroundSpecifier = table.Column<int>(type: "integer", nullable: false),
                    Background = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Team",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<int>(type: "integer", rowVersion: true, nullable: false, defaultValue: 0),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<int>(type: "integer", rowVersion: true, nullable: false, defaultValue: 0),
                    DisplayName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    ProfilePicture = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "List",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    ListId = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<int>(type: "integer", rowVersion: true, nullable: false, defaultValue: 0),
                    Title = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Order = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_List", x => new { x.ProjectId, x.ListId });
                    table.ForeignKey(
                        name: "FK_List_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTeam",
                columns: table => new
                {
                    ProjectsId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTeam", x => new { x.ProjectsId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_ProjectTeam_Project_ProjectsId",
                        column: x => x.ProjectsId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectTeam_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<int>(type: "integer", rowVersion: true, nullable: false, defaultValue: 0),
                    IdpName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    DisplayName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Role_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UploadedFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<int>(type: "integer", rowVersion: true, nullable: false, defaultValue: 0),
                    RelativePath = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                    MimeType = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    Etag = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UploadedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UploadedFile_User_UploadedById",
                        column: x => x.UploadedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "User_ResourcePolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessPolicyId = table.Column<int>(type: "integer", nullable: false),
                    ResourceType = table.Column<short>(type: "smallint", nullable: true),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: true),
                    Global = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_ResourcePolicies", x => new { x.UserId, x.Id });
                    table.ForeignKey(
                        name: "FK_User_ResourcePolicies_AccessPolicies_AccessPolicyId",
                        column: x => x.AccessPolicyId,
                        principalTable: "AccessPolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_ResourcePolicies_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Card",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<int>(type: "integer", rowVersion: true, nullable: false, defaultValue: 0),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    ListId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "Text", maxLength: 50000, nullable: false),
                    Order = table.Column<float>(type: "real", nullable: false),
                    ThumbnailId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Card", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Card_List_ProjectId_ListId",
                        columns: x => new { x.ProjectId, x.ListId },
                        principalTable: "List",
                        principalColumns: new[] { "ProjectId", "ListId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Card_User_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Role_Policies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessPolicyId = table.Column<int>(type: "integer", nullable: false),
                    ResourceType = table.Column<short>(type: "smallint", nullable: true),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: true),
                    Global = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role_Policies", x => new { x.RoleId, x.Id });
                    table.ForeignKey(
                        name: "FK_Role_Policies_AccessPolicies_AccessPolicyId",
                        column: x => x.AccessPolicyId,
                        principalTable: "AccessPolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Role_Policies_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamMember",
                columns: table => new
                {
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<int>(type: "integer", rowVersion: true, nullable: false, defaultValue: 0),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMember", x => new { x.TeamId, x.UserId });
                    table.ForeignKey(
                        name: "FK_TeamMember_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamMember_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamMember_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConvertedFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<int>(type: "integer", rowVersion: true, nullable: false, defaultValue: 0),
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

            migrationBuilder.CreateTable(
                name: "CardComment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<int>(type: "integer", rowVersion: true, nullable: false, defaultValue: 0),
                    CardId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "Text", maxLength: 50000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "FileUsage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CardId = table.Column<Guid>(type: "uuid", nullable: true),
                    UploadedFileId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: true),
                    Public = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileUsage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileUsage_Card_CardId",
                        column: x => x.CardId,
                        principalTable: "Card",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FileUsage_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id");
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

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationStore_Name",
                table: "ConfigurationStore",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConvertedFile_UploadedFileId",
                table: "ConvertedFile",
                column: "UploadedFileId");

            migrationBuilder.CreateIndex(
                name: "IX_FileUsage_CardId",
                table: "FileUsage",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_FileUsage_ProjectId",
                table: "FileUsage",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_FileUsage_Public",
                table: "FileUsage",
                column: "Public");

            migrationBuilder.CreateIndex(
                name: "IX_FileUsage_UploadedFileId",
                table: "FileUsage",
                column: "UploadedFileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTeam_TeamId",
                table: "ProjectTeam",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_TeamId",
                table: "Role",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_Policies_AccessPolicyId",
                table: "Role_Policies",
                column: "AccessPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_RoleId",
                table: "TeamMember",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_UserId",
                table: "TeamMember",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFile_UploadedById",
                table: "UploadedFile",
                column: "UploadedById");

            migrationBuilder.CreateIndex(
                name: "IX_User_ResourcePolicies_AccessPolicyId",
                table: "User_ResourcePolicies",
                column: "AccessPolicyId");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION fn_incremet_version()
                    RETURNS TRIGGER AS $$
                BEGIN
                    IF NEW.""Version"" != OLD.""Version"" THEN
                        RAISE EXCEPTION 'Concurrency version error';
                    END IF;
                    NEW.""Version"" := OLD.""Version"" + 1;
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;

                -- AccessPolicies, Role, Team, TeamMember, User, Project, List, Card, UploadedFile

                CREATE TRIGGER ""BeforeUpdateAccessPoliciesTrigger""
                    BEFORE UPDATE ON ""AccessPolicies""
                    FOR EACH ROW
                EXECUTE FUNCTION fn_incremet_version();

                CREATE TRIGGER ""BeforeUpdateRoleTrigger""
                    BEFORE UPDATE ON ""Role""
                    FOR EACH ROW
                EXECUTE FUNCTION fn_incremet_version();

                CREATE TRIGGER ""BeforeUpdateTeamTrigger""
                    BEFORE UPDATE ON ""Team""
                    FOR EACH ROW
                EXECUTE FUNCTION fn_incremet_version();

                CREATE TRIGGER ""BeforeUpdateTeamMemberTrigger""
                    BEFORE UPDATE ON ""TeamMember""
                    FOR EACH ROW
                EXECUTE FUNCTION fn_incremet_version();

                CREATE TRIGGER ""BeforeUpdateUserTrigger""
                    BEFORE UPDATE ON ""User""
                    FOR EACH ROW
                EXECUTE FUNCTION fn_incremet_version();

                CREATE TRIGGER ""BeforeUpdateProjectTrigger""
                    BEFORE UPDATE ON ""Project""
                    FOR EACH ROW
                EXECUTE FUNCTION fn_incremet_version();

                CREATE TRIGGER ""BeforeUpdateListTrigger""
                    BEFORE UPDATE ON ""List""
                    FOR EACH ROW
                EXECUTE FUNCTION fn_incremet_version();

                CREATE TRIGGER ""BeforeUpdateCardTrigger""
                    BEFORE UPDATE ON ""Card""
                    FOR EACH ROW
                EXECUTE FUNCTION fn_incremet_version();

                CREATE TRIGGER ""BeforeUpdateUploadedFileTrigger""
                    BEFORE UPDATE ON ""UploadedFile""
                    FOR EACH ROW
                EXECUTE FUNCTION fn_incremet_version();
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP TRIGGER ""BeforeUpdateAccessPoliciesTrigger"" ON ""AccessPolicies"";

                DROP TRIGGER ""BeforeUpdateRoleTrigger"" ON ""Role"";

                DROP TRIGGER ""BeforeUpdateTeamTrigger"" ON ""Team"";

                DROP TRIGGER ""BeforeUpdateTeamMemberTrigger"" ON ""TeamMember"";

                DROP TRIGGER ""BeforeUpdateUserTrigger"" ON ""User"";

                DROP TRIGGER ""BeforeUpdateProjectTrigger"" ON ""Project"";

                DROP TRIGGER ""BeforeUpdateListTrigger"" ON ""List"";

                DROP TRIGGER ""BeforeUpdateCardTrigger"" ON ""Card"";

                DROP TRIGGER ""BeforeUpdateUploadedFileTrigger"" ON ""UploadedFile"";

                DROP FUNCTION fn_incremet_version;
            ");

            migrationBuilder.DropTable(
                name: "CardComment");

            migrationBuilder.DropTable(
                name: "ConfigurationStore");

            migrationBuilder.DropTable(
                name: "ConvertedFile");

            migrationBuilder.DropTable(
                name: "FileUsage");

            migrationBuilder.DropTable(
                name: "ProjectTeam");

            migrationBuilder.DropTable(
                name: "Role_Policies");

            migrationBuilder.DropTable(
                name: "TeamMember");

            migrationBuilder.DropTable(
                name: "User_ResourcePolicies");

            migrationBuilder.DropTable(
                name: "Card");

            migrationBuilder.DropTable(
                name: "UploadedFile");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "AccessPolicies");

            migrationBuilder.DropTable(
                name: "List");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Team");

            migrationBuilder.DropTable(
                name: "Project");
        }
    }
}
