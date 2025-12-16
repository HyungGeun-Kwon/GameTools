using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTools.Server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditTriggers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Item Audit Trigger (actor + audit_skip)
            migrationBuilder.Sql(
            """
            CREATE OR ALTER TRIGGER dbo.trg_Item_Audit
            ON dbo.Item
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                -- Restore 등 특수 오퍼레이션에서 감사 억제
                IF TRY_CAST(SESSION_CONTEXT(N'audit_skip') AS bit) = 1 RETURN;

                DECLARE @now  datetime2(7) = SYSUTCDATETIME();
                DECLARE @user nvarchar(64) = TRY_CAST(SESSION_CONTEXT(N'actor') AS nvarchar(64));
                IF @user IS NULL SET @user = N'unknown';

                -- INSERT
                INSERT INTO dbo.ItemAudit (ItemId, Action, ChangedAtUtc, ChangedBy, BeforeJson, AfterJson)
                SELECT  i.Id,
                        N'INSERT',
                        @now,
                        @user,
                        NULL,
                        (SELECT i.Id, i.RarityId, i.Name, i.Price, i.Description FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
                FROM inserted i
                LEFT JOIN deleted d ON i.Id = d.Id
                WHERE d.Id IS NULL;

                -- DELETE
                INSERT INTO dbo.ItemAudit (ItemId, Action, ChangedAtUtc, ChangedBy, BeforeJson, AfterJson)
                SELECT  d.Id,
                        N'DELETE',
                        @now,
                        @user,
                        (SELECT d.Id, d.RarityId, d.Name, d.Price, d.Description FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
                        NULL
                FROM deleted d
                LEFT JOIN inserted i ON i.Id = d.Id
                WHERE i.Id IS NULL;

                -- UPDATE
                INSERT INTO dbo.ItemAudit (ItemId, Action, ChangedAtUtc, ChangedBy, BeforeJson, AfterJson)
                SELECT  i.Id,
                        N'UPDATE',
                        @now,
                        @user,
                        (SELECT d.Id, d.RarityId, d.Name, d.Price, d.Description FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
                        (SELECT i.Id, i.RarityId, i.Name, i.Price, i.Description FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
                FROM inserted i
                INNER JOIN deleted d ON i.Id = d.Id;
            END
            """);

            // Rarity Audit Trigger (actor + audit_skip)
            migrationBuilder.Sql(
            """
            CREATE OR ALTER TRIGGER dbo.trg_Rarity_Audit
            ON dbo.Rarity
            AFTER INSERT, UPDATE, DELETE
            AS
            BEGIN
                SET NOCOUNT ON;

                -- Restore 등 특수 오퍼레이션에서 감사 억제
                IF TRY_CAST(SESSION_CONTEXT(N'audit_skip') AS bit) = 1 RETURN;

                DECLARE @now  datetime2(7) = SYSUTCDATETIME();
                DECLARE @user nvarchar(64) = TRY_CAST(SESSION_CONTEXT(N'actor') AS nvarchar(64));
                IF @user IS NULL SET @user = N'unknown';

                -- INSERT
                INSERT INTO dbo.RarityAudit (RarityId, Action, ChangedAtUtc, ChangedBy, BeforeJson, AfterJson)
                SELECT  i.Id,
                        N'INSERT',
                        @now,
                        @user,
                        NULL,
                        (SELECT i.Id, i.Grade, i.ColorCode FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
                FROM inserted i
                LEFT JOIN deleted d ON i.Id = d.Id
                WHERE d.Id IS NULL;

                -- DELETE
                INSERT INTO dbo.RarityAudit (RarityId, Action, ChangedAtUtc, ChangedBy, BeforeJson, AfterJson)
                SELECT  d.Id,
                        N'DELETE',
                        @now,
                        @user,
                        (SELECT d.Id, d.Grade, d.ColorCode FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
                        NULL
                FROM deleted d
                LEFT JOIN inserted i ON i.Id = d.Id
                WHERE i.Id IS NULL;

                -- UPDATE
                INSERT INTO dbo.RarityAudit (RarityId, Action, ChangedAtUtc, ChangedBy, BeforeJson, AfterJson)
                SELECT  i.Id,
                        N'UPDATE',
                        @now,
                        @user,
                        (SELECT d.Id, d.Grade, d.ColorCode FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
                        (SELECT i.Id, i.Grade, i.ColorCode FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
                FROM inserted i
                INNER JOIN deleted d ON i.Id = d.Id;
            END
            """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS dbo.trg_Item_Audit;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS dbo.trg_Rarity_Audit;");
        }
    }
}
