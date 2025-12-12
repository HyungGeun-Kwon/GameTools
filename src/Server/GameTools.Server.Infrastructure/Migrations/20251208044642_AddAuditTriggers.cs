using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTools.Server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ... EF가 생성한 CreateTable 들 ...

            // --- Item Audit Trigger ---
            migrationBuilder.Sql(
            """
                CREATE TRIGGER [trg_Item_Audit]
                ON [dbo].[Item]
                AFTER INSERT, UPDATE, DELETE
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DECLARE @now  datetime2(7) = SYSUTCDATETIME();
                    DECLARE @user nvarchar(64) = CAST(SESSION_CONTEXT(N'CurrentUser') AS nvarchar(64));
                    IF @user IS NULL SET @user = N'unknown';

                    -- INSERT
                    INSERT INTO [dbo].[ItemAudit] (ItemId, Action, ChangedAtUtc, ChangedBy, BeforeJson, AfterJson)
                    SELECT  i.Id,
                            'INSERT',
                            @now,
                            @user,
                            NULL,
                            (
                                SELECT  i.Id,
                                        i.RarityId,
                                        i.Name,
                                        i.Price,
                                        i.Description
                                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                            )
                    FROM inserted i
                    LEFT JOIN deleted d ON i.Id = d.Id
                    WHERE d.Id IS NULL;

                    -- DELETE
                    INSERT INTO [dbo].[ItemAudit] (ItemId, Action, ChangedAtUtc, ChangedBy, BeforeJson, AfterJson)
                    SELECT  d.Id,
                            'DELETE',
                            @now,
                            @user,
                            (
                                SELECT  d.Id,
                                        d.RarityId,
                                        d.Name,
                                        d.Price,
                                        d.Description
                                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                            ),
                            NULL
                    FROM deleted d
                    LEFT JOIN inserted i ON i.Id = d.Id
                    WHERE i.Id IS NULL;

                    -- UPDATE
                    INSERT INTO [dbo].[ItemAudit] (ItemId, Action, ChangedAtUtc, ChangedBy, BeforeJson, AfterJson)
                    SELECT  i.Id,
                            'UPDATE',
                            @now,
                            @user,
                            (
                                SELECT  d.Id,
                                        d.RarityId,
                                        d.Name,
                                        d.Price,
                                        d.Description
                                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                            ),
                            (
                                SELECT  i.Id,
                                        i.RarityId,
                                        i.Name,
                                        i.Price,
                                        i.Description
                                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                            )
                    FROM inserted i
                    INNER JOIN deleted d ON i.Id = d.Id;
                END
            """);

            // --- Rarity Audit Trigger ---
            migrationBuilder.Sql(
            """
                CREATE TRIGGER [trg_Rarity_Audit]
                ON [dbo].[Rarity]
                AFTER INSERT, UPDATE, DELETE
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DECLARE @now  datetime2(7) = SYSUTCDATETIME();
                    DECLARE @user nvarchar(64) = CAST(SESSION_CONTEXT(N'CurrentUser') AS nvarchar(64));
                    IF @user IS NULL SET @user = N'unknown';

                    -- INSERT
                    INSERT INTO [dbo].[RarityAudit] (RarityId, Action, ChangedAtUtc, ChangedBy, BeforeJson, AfterJson)
                    SELECT  i.Id,
                            'INSERT',
                            @now,
                            @user,
                            NULL,
                            (
                                SELECT  i.Id,
                                        i.Grade,
                                        i.ColorCode
                                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                            )
                    FROM inserted i
                    LEFT JOIN deleted d ON i.Id = d.Id
                    WHERE d.Id IS NULL;

                    -- DELETE
                    INSERT INTO [dbo].[RarityAudit] (RarityId, Action, ChangedAtUtc, ChangedBy, BeforeJson, AfterJson)
                    SELECT  d.Id,
                            'DELETE',
                            @now,
                            @user,
                            (
                                SELECT  d.Id,
                                        d.Grade,
                                        d.ColorCode
                                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                            ),
                            NULL
                    FROM deleted d
                    LEFT JOIN inserted i ON i.Id = d.Id
                    WHERE i.Id IS NULL;

                    -- UPDATE
                    INSERT INTO [dbo].[RarityAudit] (RarityId, Action, ChangedAtUtc, ChangedBy, BeforeJson, AfterJson)
                    SELECT  i.Id,
                            'UPDATE',
                            @now,
                            @user,
                            (
                                SELECT  d.Id,
                                        d.Grade,
                                        d.ColorCode
                                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                            ),
                            (
                                SELECT  i.Id,
                                        i.Grade,
                                        i.ColorCode
                                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                            )
                    FROM inserted i
                    INNER JOIN deleted d ON i.Id = d.Id;
                END
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS [trg_Item_Audit];");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS [trg_Rarity_Audit];");

            // 나머지 DropTable은 EF가 자동 생성한 코드 그대로 두면 됨
            migrationBuilder.DropTable(name: "ItemAudit");
            migrationBuilder.DropTable(name: "RarityAudit");
            migrationBuilder.DropTable(name: "RestoreHistory");
            migrationBuilder.DropTable(name: "Item");
            migrationBuilder.DropTable(name: "Rarity");
        }
    }
}
