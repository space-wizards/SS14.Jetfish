using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SS14.Jetfish.Migrations
{
    /// <inheritdoc />
    public partial class FixListEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE
                    ""List""
                Alter COLUMN
                    ""Version""
                SET
                    DEFAULT 0;

                CREATE TRIGGER ""BeforeUpdateListTrigger""
                    BEFORE UPDATE ON ""List""
                    FOR EACH ROW
                EXECUTE FUNCTION fn_incremet_version();");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"DROP TRIGGER ""BeforeUpdateListTrigger"" ON ""List"";");
        }
    }
}
