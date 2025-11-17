using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTools.Server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRestoreToAppDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'dbo.RestoreRun', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.RestoreRun(
                        RestoreId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                        AsOfUtc datetime2 NOT NULL,
                        Actor nvarchar(128) NOT NULL,
                        DryRun bit NOT NULL,
                        StartedAtUtc datetime2 NOT NULL CONSTRAINT DF_RestoreRun_StartedAtUtc DEFAULT SYSUTCDATETIME(),
                        EndedAtUtc datetime2 NULL,
                        AffectedCounts nvarchar(max) NULL,
                        Notes nvarchar(max) NULL,
                        FiltersJson nvarchar(max) NULL
                    );
                    CREATE INDEX IX_RestoreRun_StartedAtUtc ON dbo.RestoreRun(StartedAtUtc);
                    CREATE INDEX IX_RestoreRun_DryRun ON dbo.RestoreRun(DryRun);
                    CREATE INDEX IX_RestoreRun_Actor_StartedAtUtc ON dbo.RestoreRun(Actor, StartedAtUtc);
                END
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'dbo.RestoreRun', N'U') IS NOT NULL
                    DROP TABLE dbo.RestoreRun;");
        }
    }
}
