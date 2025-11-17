using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTools.Server.Infrastructure.Migrations
{
    public partial class AddItemRestoreAsOfProc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE dbo.Item_Restore_AsOf
                    @AsOfUtc   datetime2,
                    @ItemId    int            = NULL,
                    @DryRun    bit            = 1,
                    @Actor     nvarchar(128)  = NULL,
                    @Notes     nvarchar(4000) = NULL
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SET XACT_ABORT ON;

                    DECLARE @RestoreId uniqueidentifier = NEWID();
                    DECLARE @actorName nvarchar(128) =
                      COALESCE(@Actor, TRY_CAST(SESSION_CONTEXT(N'actor') AS nvarchar(128)), SUSER_SNAME());

                    -- 실행 헤더
                    INSERT dbo.RestoreRun(RestoreId, AsOfUtc, Actor, DryRun, Notes, FiltersJson)
                    VALUES (@RestoreId, @AsOfUtc, @actorName, @DryRun, @Notes,
                            (SELECT @ItemId AS ItemId FOR JSON PATH, WITHOUT_ARRAY_WRAPPER));

                    BEGIN TRAN;

                    -- 동시 실행 방지
                    DECLARE @lockResult int;
                    EXEC @lockResult = sys.sp_getapplock
                        @Resource    = N'Item_Restore_AsOf',
                        @LockMode    = N'Exclusive',
                        @LockOwner   = N'Transaction',
                        @LockTimeout = 30000,
                        @DbPrincipal = N'public';
                    IF (@lockResult < 0)
                    BEGIN
                        ROLLBACK;
                        RAISERROR('Item_Restore_AsOf: failed to acquire applock (timeout)', 16, 1);
                        RETURN;
                    END

                    -- 1) 대상 감사 로그 스냅샷(AsOf 이후) - 정적 분기 (동적 SQL 제거)
                    CREATE TABLE #Logs
                    (
                        AuditId      bigint        NULL,
                        ItemId       int           NOT NULL,
                        Action       varchar(10)   NOT NULL,
                        BeforeJson   nvarchar(max) NULL,
                        AfterJson    nvarchar(max) NULL,
                        ChangedAtUtc datetime2     NOT NULL
                    );

                    IF EXISTS (SELECT 1
                               FROM sys.columns
                               WHERE object_id = OBJECT_ID(N'dbo.ItemAudit')
                                 AND name = N'AuditId')
                    BEGIN
                        INSERT #Logs(AuditId, ItemId, Action, BeforeJson, AfterJson, ChangedAtUtc)
                        SELECT CAST(AuditId AS bigint), ItemId, Action, BeforeJson, AfterJson, ChangedAtUtc
                        FROM dbo.ItemAudit WITH (READCOMMITTEDLOCK)
                        WHERE ChangedAtUtc > @AsOfUtc
                          AND (@ItemId IS NULL OR ItemId = @ItemId);
                    END
                    ELSE
                    BEGIN
                        INSERT #Logs(AuditId, ItemId, Action, BeforeJson, AfterJson, ChangedAtUtc)
                        SELECT CAST(NULL AS bigint), ItemId, Action, BeforeJson, AfterJson, ChangedAtUtc
                        FROM dbo.ItemAudit WITH (READCOMMITTEDLOCK)
                        WHERE ChangedAtUtc > @AsOfUtc
                          AND (@ItemId IS NULL OR ItemId = @ItemId);
                    END

                    IF NOT EXISTS (SELECT 1 FROM #Logs)
                    BEGIN
                        UPDATE dbo.RestoreRun
                           SET EndedAtUtc = SYSUTCDATETIME(),
                               AffectedCounts = N'{""Delete"":0,""Insert"":0,""Update"":0}'
                         WHERE RestoreId = @RestoreId;

                        COMMIT;

                        SELECT @RestoreId AS RestoreId, 0 AS Deleted, 0 AS Inserted, 0 AS Updated, CAST(1 AS bit) AS IsChanged;
                        RETURN;
                    END

                    -- 2) 결과 수집 & 감사 억제
                    DECLARE @DelApplied TABLE(Id int PRIMARY KEY);
                    DECLARE @InsApplied TABLE(Id int PRIMARY KEY);
                    DECLARE @UpdApplied TABLE(Id int PRIMARY KEY);
                    DECLARE @identityOn bit = 0;

                    EXEC sys.sp_set_session_context @key=N'audit_skip',   @value=1;
                    EXEC sys.sp_set_session_context @key=N'txid',         @value=@RestoreId;
                    EXEC sys.sp_set_session_context @key=N'audit_reason', @value=N'RESTORE';

                    -- 3) 최신→과거 역순 1건씩 역연산
                    DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
                    SELECT Action, BeforeJson, AfterJson,
                           COALESCE(TRY_CAST(JSON_VALUE(BeforeJson,'$.Id') AS int),
                                    TRY_CAST(JSON_VALUE(AfterJson,'$.Id')  AS int)) AS Id,
                           ChangedAtUtc,
                           ISNULL(AuditId, 0) AS AuditId
                    FROM #Logs
                    ORDER BY ChangedAtUtc DESC, AuditId DESC;

                    DECLARE @act varchar(10), @bj nvarchar(max), @aj nvarchar(max), @id int, @ts datetime2, @aid bigint;

                    BEGIN TRY
                        OPEN cur;
                        FETCH NEXT FROM cur INTO @act, @bj, @aj, @id, @ts, @aid;

                        WHILE @@FETCH_STATUS = 0
                        BEGIN
                            IF @act = 'INSERT'
                            BEGIN
                                DELETE FROM dbo.Item WHERE Id = @id;
                                IF @@ROWCOUNT > 0 AND NOT EXISTS (SELECT 1 FROM @DelApplied WHERE Id=@id)
                                    INSERT INTO @DelApplied(Id) VALUES(@id);
                            END
                            ELSE IF @act = 'DELETE'
                            BEGIN
                                IF @bj IS NULL OR ISJSON(@bj) <> 1
                                BEGIN
                                    FETCH NEXT FROM cur INTO @act, @bj, @aj, @id, @ts, @aid; CONTINUE;
                                END

                                DECLARE @Name nvarchar(100), @Description nvarchar(4000);
                                DECLARE @Price int, @RarityId tinyint, @IdFromJson int;

                                SELECT @IdFromJson = Id, @Name = Name, @Price = Price,
                                       @Description = Description, @RarityId = RarityId
                                FROM OPENJSON(@bj)
                                WITH (
                                    Id          int             '$.Id',
                                    Name        nvarchar(100)   '$.Name',
                                    Price       int             '$.Price',
                                    Description nvarchar(4000)  '$.Description',
                                    RarityId    tinyint         '$.RarityId'
                                );

                                IF @IdFromJson IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Item WHERE Id=@IdFromJson)
                                BEGIN
                                    IF @identityOn = 0
                                    BEGIN
                                        SET IDENTITY_INSERT dbo.Item ON;
                                        SET @identityOn = 1;
                                    END

                                    INSERT dbo.Item(Id, Name, Price, Description, RarityId)
                                    VALUES(@IdFromJson, @Name, @Price, @Description, @RarityId);

                                    IF NOT EXISTS (SELECT 1 FROM @InsApplied WHERE Id=@IdFromJson)
                                        INSERT INTO @InsApplied(Id) VALUES(@IdFromJson);
                                END
                            END
                            ELSE IF @act = 'UPDATE'
                            BEGIN
                                IF @bj IS NULL OR ISJSON(@bj) <> 1
                                BEGIN
                                    FETCH NEXT FROM cur INTO @act, @bj, @aj, @id, @ts, @aid; CONTINUE;
                                END

                                DECLARE @UName nvarchar(100), @UDescription nvarchar(4000);
                                DECLARE @UPrice int, @URarityId tinyint, @UId int;

                                SELECT @UId = Id, @UName = Name, @UPrice = Price,
                                       @UDescription = Description, @URarityId = RarityId
                                FROM OPENJSON(@bj)
                                WITH (
                                    Id          int             '$.Id',
                                    Name        nvarchar(100)   '$.Name',
                                    Price       int             '$.Price',
                                    Description nvarchar(4000)  '$.Description',
                                    RarityId    tinyint         '$.RarityId'
                                );

                                IF @UId IS NOT NULL
                                BEGIN
                                    UPDATE t
                                       SET Name = @UName,
                                           Price = @UPrice,
                                           Description = @UDescription,
                                           RarityId = @URarityId
                                    FROM dbo.Item t
                                    WHERE t.Id = @UId;

                                    IF @@ROWCOUNT > 0 AND NOT EXISTS (SELECT 1 FROM @UpdApplied WHERE Id=@UId)
                                        INSERT INTO @UpdApplied(Id) VALUES(@UId);
                                END
                            END

                            FETCH NEXT FROM cur INTO @act, @bj, @aj, @id, @ts, @aid;
                        END

                        CLOSE cur; DEALLOCATE cur;
                    END TRY
                    BEGIN CATCH
                        -- 커서/IDENTITY_INSERT/세션컨텍스트 정리 후 롤백
                        BEGIN TRY CLOSE cur; END TRY BEGIN CATCH END CATCH;
                        BEGIN TRY DEALLOCATE cur; END TRY BEGIN CATCH END CATCH;

                        IF @identityOn = 1
                        BEGIN
                            BEGIN TRY SET IDENTITY_INSERT dbo.Item OFF; END TRY BEGIN CATCH END CATCH;
                            SET @identityOn = 0;
                        END

                        BEGIN TRY
                            EXEC sys.sp_set_session_context @key=N'audit_skip',   @value=NULL;
                            EXEC sys.sp_set_session_context @key=N'txid',         @value=NULL;
                            EXEC sys.sp_set_session_context @key=N'audit_reason', @value=NULL;
                        END TRY BEGIN CATCH END CATCH;

                        IF XACT_STATE() <> 0 ROLLBACK;
                        THROW;
                    END CATCH

                    -- 집계
                    DECLARE @cd int = (SELECT COUNT(*) FROM @DelApplied);
                    DECLARE @ci int = (SELECT COUNT(*) FROM @InsApplied);
                    DECLARE @cu int = (SELECT COUNT(*) FROM @UpdApplied);

                    -- 커밋 경로에서도 IDENTITY_INSERT OFF 보장
                    IF @identityOn = 1
                    BEGIN
                        BEGIN TRY SET IDENTITY_INSERT dbo.Item OFF; END TRY BEGIN CATCH END CATCH;
                        SET @identityOn = 0;
                    END

                    -- 커밋/롤백
                    IF @DryRun = 1
                        ROLLBACK;
                    ELSE
                        COMMIT;

                    -- 세션 컨텍스트 정리
                    BEGIN TRY
                        EXEC sys.sp_set_session_context @key=N'audit_skip',   @value=NULL;
                        EXEC sys.sp_set_session_context @key=N'txid',         @value=NULL;
                        EXEC sys.sp_set_session_context @key=N'audit_reason', @value=NULL;
                    END TRY BEGIN CATCH END CATCH;

                    -- 헤더 총계/종료
                    UPDATE dbo.RestoreRun
                       SET EndedAtUtc = SYSUTCDATETIME(),
                           AffectedCounts = CONCAT('{""Delete"":', @cd, ',""Insert"":', @ci, ',""Update"":', @cu, '}')
                     WHERE RestoreId = @RestoreId;

                    -- 결과 1셋: 총계
                    SELECT @RestoreId AS RestoreId, @cd AS Deleted, @ci AS Inserted, @cu AS Updated,
                           CAST(CASE WHEN (@cd+@ci+@cu) > 0 THEN 1 ELSE 0 END AS bit) AS IsChanged;

                    -- 결과 2셋: 실행 시에만 상세 목록
                    IF (@DryRun = 0)
                    BEGIN
                        SELECT 'DELETE' AS Op, Id FROM @DelApplied
                        UNION ALL SELECT 'INSERT', Id FROM @InsApplied
                        UNION ALL SELECT 'UPDATE', Id FROM @UpdApplied
                        ORDER BY Op, Id;
                    END
                END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF OBJECT_ID(N'dbo.Item_Restore_AsOf', 'P') IS NOT NULL DROP PROCEDURE dbo.Item_Restore_AsOf;");
        }
    }
}
