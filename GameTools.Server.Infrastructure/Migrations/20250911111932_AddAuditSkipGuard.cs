using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTools.Server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditSkipGuard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Item 감사 트리거: audit_skip 가드 추가
            migrationBuilder.Sql(@"
                CREATE OR ALTER TRIGGER dbo.trg_Item_Audit
                ON dbo.Item
                AFTER INSERT, UPDATE, DELETE
                AS
                BEGIN
                  SET NOCOUNT ON;
                  IF TRY_CAST(SESSION_CONTEXT(N'audit_skip') AS bit) = 1 RETURN;

                  INSERT INTO dbo.ItemAudit
                  (
                    ItemId, Action, BeforeJson, AfterJson, ChangedAtUtc, ChangedBy
                  )
                  SELECT
                    COALESCE(d.Id, i.Id),
                    CASE WHEN d.Id IS NULL THEN 'INSERT'
                         WHEN i.Id IS NULL THEN 'DELETE' ELSE 'UPDATE' END,
                    CASE WHEN d.Id IS NOT NULL
                         THEN (SELECT d.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) END,
                    CASE WHEN i.Id IS NOT NULL
                         THEN (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) END,
                    SYSUTCDATETIME(),
                    COALESCE(TRY_CAST(SESSION_CONTEXT(N'actor') AS nvarchar(64)), SYSTEM_USER)
                  FROM inserted i
                  FULL OUTER JOIN deleted d ON d.Id = i.Id;
                END;
            ");

            // Rarity 감사 트리거: audit_skip 가드 추가
            migrationBuilder.Sql(@"
                CREATE OR ALTER TRIGGER dbo.trg_Rarity_Audit
                ON dbo.Rarity
                AFTER INSERT, UPDATE, DELETE
                AS
                BEGIN
                  SET NOCOUNT ON;
                  IF TRY_CAST(SESSION_CONTEXT(N'audit_skip') AS bit) = 1 RETURN;

                  INSERT INTO dbo.RarityAudit
                  (
                    RarityId, Action, BeforeJson, AfterJson, ChangedAtUtc, ChangedBy
                  )
                  SELECT
                    COALESCE(d.Id, i.Id),
                    CASE WHEN d.Id IS NULL THEN 'INSERT'
                         WHEN i.Id IS NULL THEN 'DELETE' ELSE 'UPDATE' END,
                    CASE WHEN d.Id IS NOT NULL
                         THEN (SELECT d.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) END,
                    CASE WHEN i.Id IS NOT NULL
                         THEN (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) END,
                    SYSUTCDATETIME(),
                    COALESCE(TRY_CAST(SESSION_CONTEXT(N'actor') AS nvarchar(64)), SYSTEM_USER)
                  FROM inserted i
                  FULL OUTER JOIN deleted d ON d.Id = i.Id;
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Item 감사 트리거: audit_skip 가드 제거(원본으로 복구)
            migrationBuilder.Sql(@"
                CREATE OR ALTER TRIGGER dbo.trg_Item_Audit
                ON dbo.Item
                AFTER INSERT, UPDATE, DELETE
                AS
                BEGIN
                  SET NOCOUNT ON;

                  INSERT INTO dbo.ItemAudit
                  (
                    ItemId, Action, BeforeJson, AfterJson, ChangedAtUtc, ChangedBy
                  )
                  SELECT
                    COALESCE(d.Id, i.Id),
                    CASE WHEN d.Id IS NULL THEN 'INSERT'
                         WHEN i.Id IS NULL THEN 'DELETE' ELSE 'UPDATE' END,
                    CASE WHEN d.Id IS NOT NULL
                         THEN (SELECT d.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) END,
                    CASE WHEN i.Id IS NOT NULL
                         THEN (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) END,
                    SYSUTCDATETIME(),
                    COALESCE(TRY_CAST(SESSION_CONTEXT(N'actor') AS nvarchar(64)), SYSTEM_USER)
                  FROM inserted i
                  FULL OUTER JOIN deleted d ON d.Id = i.Id;
                END;
            ");

            // Rarity 감사 트리거: audit_skip 가드 제거(원본으로 복구)
            migrationBuilder.Sql(@"
                CREATE OR ALTER TRIGGER dbo.trg_Rarity_Audit
                ON dbo.Rarity
                AFTER INSERT, UPDATE, DELETE
                AS
                BEGIN
                  SET NOCOUNT ON;

                  INSERT INTO dbo.RarityAudit
                  (
                    RarityId, Action, BeforeJson, AfterJson, ChangedAtUtc, ChangedBy
                  )
                  SELECT
                    COALESCE(d.Id, i.Id),
                    CASE WHEN d.Id IS NULL THEN 'INSERT'
                         WHEN i.Id IS NULL THEN 'DELETE' ELSE 'UPDATE' END,
                    CASE WHEN d.Id IS NOT NULL
                         THEN (SELECT d.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) END,
                    CASE WHEN i.Id IS NOT NULL
                         THEN (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) END,
                    SYSUTCDATETIME(),
                    COALESCE(TRY_CAST(SESSION_CONTEXT(N'actor') AS nvarchar(64)), SYSTEM_USER)
                  FROM inserted i
                  FULL OUTER JOIN deleted d ON d.Id = i.Id;
                END;
            ");
        }
    }
}
