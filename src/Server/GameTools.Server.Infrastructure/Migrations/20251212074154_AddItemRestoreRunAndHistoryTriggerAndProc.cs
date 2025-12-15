using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTools.Server.Infrastructure.Migrations
{
    public partial class AddItemRestoreRunAndHistoryTriggerAndProc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
            """
            CREATE OR ALTER PROCEDURE dbo.usp_ItemRestore_AsOf
                @AsOfUtc     datetime2(7),
                @ItemIdsJson nvarchar(max) = NULL, -- NULL: 전체, JSON array: 특정 아이템들
                @DryRun      bit           = 1,
                @Notes       nvarchar(max) = NULL
            AS
            BEGIN
                SET NOCOUNT ON;
                SET XACT_ABORT ON;

                DECLARE @RestoreId uniqueidentifier = NEWID();
                DECLARE @actorName nvarchar(128) =
                    COALESCE(TRY_CAST(SESSION_CONTEXT(N'actor') AS nvarchar(128)), SUSER_SNAME());

                DECLARE @filters nvarchar(max) =
                    CASE
                        WHEN @ItemIdsJson IS NULL THEN N'{"ItemIds":null}'
                        WHEN ISJSON(@ItemIdsJson) = 1 THEN N'{"ItemIds":' + @ItemIdsJson + N'}'
                        ELSE N'{"ItemIdsRaw":"' + REPLACE(@ItemIdsJson, '"', '\"') + N'"}'
                    END;

                -- 0) RestoreHistory "헤더" 기록 (트랜잭션 밖: DryRun이어도 기록 남기기)
                INSERT dbo.RestoreHistory (RestoreId, AsOfUtc, Actor, DryRun, Notes, FiltersJson)
                VALUES (@RestoreId, @AsOfUtc, @actorName, @DryRun, @Notes, @filters);

                BEGIN TRAN;

                -- 1) 동시 실행 방지
                DECLARE @lockResult int;
                EXEC @lockResult = sys.sp_getapplock
                    @Resource    = N'usp_ItemRestore_AsOf',
                    @LockMode    = N'Exclusive',
                    @LockOwner   = N'Transaction',
                    @LockTimeout = 30000,
                    @DbPrincipal = N'public';

                IF (@lockResult < 0)
                BEGIN
                    ROLLBACK;
                    UPDATE dbo.RestoreHistory
                       SET EndedAtUtc = SYSUTCDATETIME(),
                           AffectedCounts = N'{"Delete":0,"Insert":0,"Update":0}'
                     WHERE RestoreId = @RestoreId;

                    RAISERROR('usp_ItemRestore_AsOf: failed to acquire applock (timeout)', 16, 1);
                    RETURN;
                END

                -- 2) 대상 ItemId 집합 (옵션)
                DECLARE @TargetIds TABLE (Id uniqueidentifier NOT NULL PRIMARY KEY);

                IF (@ItemIdsJson IS NOT NULL AND ISJSON(@ItemIdsJson) = 1)
                BEGIN
                    INSERT INTO @TargetIds(Id)
                    SELECT TRY_CAST([value] AS uniqueidentifier)
                    FROM OPENJSON(@ItemIdsJson)
                    WHERE TRY_CAST([value] AS uniqueidentifier) IS NOT NULL;
                END

                -- 3) AsOf 이후 감사 로그 스냅샷
                CREATE TABLE #Logs
                (
                    AuditId      uniqueidentifier NOT NULL,
                    ItemId       uniqueidentifier NOT NULL,
                    Action       nvarchar(10)     NOT NULL,
                    BeforeJson   nvarchar(max)    NULL,
                    AfterJson    nvarchar(max)    NULL,
                    ChangedAtUtc datetime2(7)     NOT NULL
                );

                INSERT #Logs(AuditId, ItemId, Action, BeforeJson, AfterJson, ChangedAtUtc)
                SELECT a.AuditId, a.ItemId, a.Action, a.BeforeJson, a.AfterJson, a.ChangedAtUtc
                FROM dbo.ItemAudit a WITH (READCOMMITTEDLOCK)
                WHERE a.ChangedAtUtc > @AsOfUtc
                  AND (
                        @ItemIdsJson IS NULL
                        OR EXISTS (SELECT 1 FROM @TargetIds t WHERE t.Id = a.ItemId)
                  );

                IF NOT EXISTS (SELECT 1 FROM #Logs)
                BEGIN
                    IF @DryRun = 1 ROLLBACK ELSE COMMIT;

                    UPDATE dbo.RestoreHistory
                       SET EndedAtUtc = SYSUTCDATETIME(),
                           AffectedCounts = N'{"Delete":0,"Insert":0,"Update":0}'
                     WHERE RestoreId = @RestoreId;

                    SELECT @RestoreId AS RestoreId, 0 AS Deleted, 0 AS Inserted, 0 AS Updated,
                           CAST(0 AS bit) AS IsChanged;
                    RETURN;
                END

                -- 4) 복구 적용 중 감사 억제
                EXEC sys.sp_set_session_context @key=N'audit_skip', @value=1;

                DECLARE @DelApplied TABLE(Id uniqueidentifier PRIMARY KEY);
                DECLARE @InsApplied TABLE(Id uniqueidentifier PRIMARY KEY);
                DECLARE @UpdApplied TABLE(Id uniqueidentifier PRIMARY KEY);

                -- 5) 최신 -> 과거 순으로 역연산
                DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
                SELECT Action, BeforeJson, AfterJson, ItemId, ChangedAtUtc, AuditId
                FROM #Logs
                ORDER BY ChangedAtUtc DESC, AuditId DESC;

                DECLARE
                    @act nvarchar(10),
                    @bj nvarchar(max),
                    @aj nvarchar(max),
                    @id uniqueidentifier,
                    @ts datetime2(7),
                    @aid uniqueidentifier;

                BEGIN TRY
                    OPEN cur;
                    FETCH NEXT FROM cur INTO @act, @bj, @aj, @id, @ts, @aid;

                    WHILE @@FETCH_STATUS = 0
                    BEGIN
                        IF @act = N'INSERT'
                        BEGIN
                            DELETE FROM dbo.Item WHERE Id = @id;
                            IF @@ROWCOUNT > 0 AND NOT EXISTS (SELECT 1 FROM @DelApplied WHERE Id=@id)
                                INSERT INTO @DelApplied(Id) VALUES(@id);
                        END
                        ELSE IF @act = N'DELETE'
                        BEGIN
                            IF @bj IS NOT NULL AND ISJSON(@bj) = 1
                            BEGIN
                                DECLARE @Name nvarchar(100), @Desc nvarchar(1000), @Price int, @RarityId uniqueidentifier, @JsonId uniqueidentifier;

                                SELECT
                                    @JsonId = Id,
                                    @Name = Name,
                                    @Price = Price,
                                    @Desc = Description,
                                    @RarityId = RarityId
                                FROM OPENJSON(@bj)
                                WITH (
                                    Id          uniqueidentifier '$.Id',
                                    Name        nvarchar(100)   '$.Name',
                                    Price       int             '$.Price',
                                    Description nvarchar(1000)  '$.Description',
                                    RarityId    uniqueidentifier '$.RarityId'
                                );

                                IF @JsonId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Item WHERE Id=@JsonId)
                                BEGIN
                                    INSERT dbo.Item(Id, Name, Price, Description, RarityId)
                                    VALUES(@JsonId, @Name, @Price, @Desc, @RarityId);

                                    IF NOT EXISTS (SELECT 1 FROM @InsApplied WHERE Id=@JsonId)
                                        INSERT INTO @InsApplied(Id) VALUES(@JsonId);
                                END
                            END
                        END
                        ELSE IF @act = N'UPDATE'
                        BEGIN
                            IF @bj IS NOT NULL AND ISJSON(@bj) = 1
                            BEGIN
                                DECLARE @UName nvarchar(100), @UDesc nvarchar(1000), @UPrice int, @URarityId uniqueidentifier, @UId uniqueidentifier;

                                SELECT
                                    @UId = Id,
                                    @UName = Name,
                                    @UPrice = Price,
                                    @UDesc = Description,
                                    @URarityId = RarityId
                                FROM OPENJSON(@bj)
                                WITH (
                                    Id          uniqueidentifier '$.Id',
                                    Name        nvarchar(100)   '$.Name',
                                    Price       int             '$.Price',
                                    Description nvarchar(1000)  '$.Description',
                                    RarityId    uniqueidentifier '$.RarityId'
                                );

                                IF @UId IS NOT NULL
                                BEGIN
                                    UPDATE t
                                       SET Name = @UName,
                                           Price = @UPrice,
                                           Description = @UDesc,
                                           RarityId = @URarityId
                                    FROM dbo.Item t
                                    WHERE t.Id = @UId;

                                    IF @@ROWCOUNT > 0 AND NOT EXISTS (SELECT 1 FROM @UpdApplied WHERE Id=@UId)
                                        INSERT INTO @UpdApplied(Id) VALUES(@UId);
                                END
                            END
                        END

                        FETCH NEXT FROM cur INTO @act, @bj, @aj, @id, @ts, @aid;
                    END

                    CLOSE cur;
                    DEALLOCATE cur;
                END TRY
                BEGIN CATCH
                    BEGIN TRY CLOSE cur; END TRY BEGIN CATCH END CATCH;
                    BEGIN TRY DEALLOCATE cur; END TRY BEGIN CATCH END CATCH;

                    BEGIN TRY EXEC sys.sp_set_session_context @key=N'audit_skip', @value=NULL; END TRY BEGIN CATCH END CATCH;

                    IF XACT_STATE() <> 0 ROLLBACK;

                    UPDATE dbo.RestoreHistory
                       SET EndedAtUtc = SYSUTCDATETIME(),
                           AffectedCounts = N'{"Delete":0,"Insert":0,"Update":0}'
                     WHERE RestoreId = @RestoreId;

                    THROW;
                END CATCH

                DECLARE @cd int = (SELECT COUNT(*) FROM @DelApplied);
                DECLARE @ci int = (SELECT COUNT(*) FROM @InsApplied);
                DECLARE @cu int = (SELECT COUNT(*) FROM @UpdApplied);

                IF @DryRun = 1
                    ROLLBACK;
                ELSE
                    COMMIT;

                BEGIN TRY EXEC sys.sp_set_session_context @key=N'audit_skip', @value=NULL; END TRY BEGIN CATCH END CATCH;

                UPDATE dbo.RestoreHistory
                   SET EndedAtUtc = SYSUTCDATETIME(),
                       AffectedCounts = CONCAT(N'{"Delete":', @cd, N',"Insert":', @ci, N',"Update":', @cu, N'}')
                 WHERE RestoreId = @RestoreId;

                SELECT
                    @RestoreId AS RestoreId,
                    @cd AS Deleted,
                    @ci AS Inserted,
                    @cu AS Updated,
                    CAST(CASE WHEN (@cd + @ci + @cu) > 0 THEN 1 ELSE 0 END AS bit) AS IsChanged;
            END
            """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
            """
            IF OBJECT_ID(N'dbo.usp_ItemRestore_AsOf', 'P') IS NOT NULL
                DROP PROCEDURE dbo.usp_ItemRestore_AsOf;
            """);
        }
    }
}
