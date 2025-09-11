using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTools.Server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRestoreRunTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RestoreRun",
                columns: table => new
                {
                    RestoreId = table.Column<Guid>("uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    AsOfUtc = table.Column<DateTime>("datetime2", nullable: false),
                    Actor = table.Column<string>("nvarchar(128)", maxLength: 128, nullable: false),
                    FiltersJson = table.Column<string>("nvarchar(max)", nullable: true),
                    DryRun = table.Column<bool>("bit", nullable: false),
                    StartedAtUtc = table.Column<DateTime>("datetime2", nullable: false, defaultValueSql: "sysutcdatetime()"),
                    EndedAtUtc = table.Column<DateTime>("datetime2", nullable: true),
                    AffectedCounts = table.Column<string>("nvarchar(4000)", maxLength: 4000, nullable: true),
                    Notes = table.Column<string>("nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestoreRun", x => x.RestoreId);
                });

            migrationBuilder.CreateTable(
                name: "RestoreRunDetail",
                columns: table => new
                {
                    RestoreId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    AuditId = table.Column<long>("bigint", nullable: false),
                    TargetTable = table.Column<string>("nvarchar(128)", maxLength: 128, nullable: false),
                    TargetKey = table.Column<string>("nvarchar(256)", maxLength: 256, nullable: false),
                    ReverseOp = table.Column<string>("varchar(8)", nullable: false),
                    Outcome = table.Column<string>("varchar(16)", nullable: false),
                    Message = table.Column<string>("nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestoreRunDetail", x => new { x.RestoreId, x.AuditId });
                    table.ForeignKey(
                        name: "FK_RestoreRunDetail_RestoreRun_RestoreId",
                        column: x => x.RestoreId,
                        principalTable: "RestoreRun",
                        principalColumn: "RestoreId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestoreRun_StartedAtUtc",
                table: "RestoreRun",
                column: "StartedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_RestoreRunDetail_Target",
                table: "RestoreRunDetail",
                columns: ["TargetTable", "TargetKey"]);

            migrationBuilder.CreateIndex(
                name: "IX_RestoreRunDetail_Outcome",
                table: "RestoreRunDetail",
                column: "Outcome");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "RestoreRunDetail");
            migrationBuilder.DropTable(name: "RestoreRun");
        }
    }
}
