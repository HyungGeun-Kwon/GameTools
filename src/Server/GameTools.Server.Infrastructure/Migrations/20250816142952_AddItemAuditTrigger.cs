using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTools.Server.Infrastructure.Migrations
{
    public partial class AddItemAuditTrigger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF OBJECT_ID(N'dbo.trg_Item_Audit','TR') IS NOT NULL DROP TRIGGER dbo.trg_Item_Audit;");
        }
    }
}
