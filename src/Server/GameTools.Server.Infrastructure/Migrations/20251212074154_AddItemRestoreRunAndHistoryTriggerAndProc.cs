using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTools.Server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddItemRestoreRunAndHistoryTriggerAndProc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) RestoreRun 테이블 (RestoreHistory와 거의 동일, 내부 실행 로그용)
            migrationBuilder.Sql(
            """
            IF OBJECT_ID(N'dbo.RestoreRun', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.RestoreRun
                (
                    RestoreId      uniqueidentifier NOT NULL CONSTRAINT PK_RestoreRun PRIMARY KEY,
                    AsOfUtc        datetime2(7)     NOT NULL,
                    Actor          nvarchar(128)    NOT NULL CONSTRAINT DF_RestoreRun_Actor DEFAULT N'unknown',
                    DryRun         bit              NOT NULL,
                    StartedAtUtc   datetime2(7)     NOT NULL CONSTRAINT DF_RestoreRun_StartedAtUtc DEFAULT SYSUTCDATETIME(),
                    EndedAtUtc     datetime2(7)     NULL,
                    AffectedCounts nvarchar(max)    NULL,
                    Notes          nvarchar(max)    NULL,
                    FiltersJson    nvarchar(max)    NULL
                );

                CREATE INDEX IX_RestoreRun_StartedAtUtc ON dbo.RestoreRun(StartedAtUtc);
                CREATE INDEX IX_RestoreRun_DryRun ON dbo.RestoreRun(DryRun);
                CREATE INDEX IX_RestoreRun_Actor_StartedAtUtc ON dbo.RestoreRun(Actor, StartedAtUtc);
            END
            """);

            // 2) RestoreRun -> RestoreHistory 동기화 트리거
            //    - Insert: RestoreHistory에 Insert (없으면)
            //    - Update: RestoreHistory에 Update (EndedAtUtc, AffectedCounts 등 반영)
            migrationBuilder.Sql(
            """
            CREATE OR ALTER TRIGGER dbo.trg_RestoreRun_ToHistory
            ON dbo.RestoreRun
            AFTER INSERT, UPDATE
            AS
            BEGIN
                SET NOCOUNT ON;

                -- INSERT 된 행들: RestoreHistory에 없으면 생성
                INSERT INTO dbo.RestoreHistory
                (
                    RestoreId, AsOfUtc, Actor, DryRun, StartedAtUtc, EndedAtUtc,
                    AffectedCounts, Notes, FiltersJson
                )
                SELECT
                    i.RestoreId, i.AsOfUtc, i.Actor, i.DryRun, i.StartedAtUtc, i.EndedAtUtc,
                    i.AffectedCounts, i.Notes, i.FiltersJson
                FROM inserted i
                WHERE NOT EXISTS (
                    SELECT 1 FROM dbo.RestoreHistory h WHERE h.RestoreId = i.RestoreId
                );

                -- UPDATE 된 행들: RestoreHistory에 반영
                UPDATE h
                SET
                    h.AsOfUtc        = i.AsOfUtc,
                    h.Actor          = i.Actor,
                    h.DryRun         = i.DryRun,
                    h.StartedAtUtc   = i.StartedAtUtc,
                    h.EndedAtUtc     = i.EndedAtUtc,
                    h.AffectedCounts = i.AffectedCounts,
                    h.Notes          = i.Notes,
                    h.FiltersJson    = i.FiltersJson
                FROM dbo.RestoreHistory h
                JOIN inserted i ON i.RestoreId = h.RestoreId;
            END
            """);

            migrationBuilder.Sql(
            """
            IF OBJECT_ID(N'dbo.trg_Item_Audit', 'TR') IS NOT NULL
            BEGIN
                EXEC(N'
                CREATE OR ALTER TRIGGER dbo.trg_Item_Audit
                ON dbo.Item
                AFTER INSERT, UPDATE, DELETE
                AS
                BEGIN
                  SET NOCOUNT ON;
                  IF TRY_CAST(SESSION_CONTEXT(N''audit_skip'') AS bit) = 1 RETURN;

                  INSERT INTO dbo.ItemAudit (ItemId, Action, BeforeJson, AfterJson, ChangedAtUtc, ChangedBy)
                  SELECT
                    COALESCE(d.Id, i.Id),
                    CASE WHEN d.Id IS NULL THEN ''INSERT''
                         WHEN i.Id IS NULL THEN ''DELETE'' ELSE ''UPDATE'' END,
                    CASE WHEN d.Id IS NOT NULL
                         THEN (SELECT d.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) END,
                    CASE WHEN i.Id IS NOT NULL
                         THEN (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) END,
                    SYSUTCDATETIME(),
                    COALESCE(TRY_CAST(SESSION_CONTEXT(N''CurrentUser'') AS nvarchar(64)), SYSTEM_USER)
                  FROM inserted i
                  FULL OUTER JOIN deleted d ON d.Id = i.Id;
                END;
                ');
            END

            -- Rarity 감사 트리거: ChangedBy를 Actor로
            IF OBJECT_ID(N'dbo.trg_Rarity_Audit', 'TR') IS NOT NULL
            BEGIN
                EXEC(N'
                CREATE OR ALTER TRIGGER dbo.trg_Rarity_Audit
                ON dbo.Rarity
                AFTER INSERT, UPDATE, DELETE
                AS
                BEGIN
                  SET NOCOUNT ON;
                  IF TRY_CAST(SESSION_CONTEXT(N''audit_skip'') AS bit) = 1 RETURN;

                  INSERT INTO dbo.RarityAudit (RarityId, Action, BeforeJson, AfterJson, ChangedAtUtc, ChangedBy)
                  SELECT
                    COALESCE(d.Id, i.Id),
                    CASE WHEN d.Id IS NULL THEN ''INSERT''
                         WHEN i.Id IS NULL THEN ''DELETE'' ELSE ''UPDATE'' END,
                    CASE WHEN d.Id IS NOT NULL
                         THEN (SELECT d.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) END,
                    CASE WHEN i.Id IS NOT NULL
                         THEN (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) END,
                    SYSUTCDATETIME(),
                    COALESCE(TRY_CAST(SESSION_CONTEXT(N''Actor'') AS nvarchar(64)), SYSTEM_USER)
                  FROM inserted i
                  FULL OUTER JOIN deleted d ON d.Id = i.Id;
                END;
                ');
            END
            """);

            // 4) Item Restore AsOf 프로시저 (Guid 기반, ItemAudit 스키마에 맞춤)
            //    - RestoreRun에 기록 -> 트리거가 RestoreHistory 반영
            //    - audit_skip=1로 감사 억제
            //    - applock으로 동시 실행 방지
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

                DECLARE @RestoreId uniqueidentifier = NEWSEQUENTIALID();
                DECLARE @currentUserName nvarchar(128) =
                    COALESCE(TRY_CAST(SESSION_CONTEXT(N'CurrentUser') AS nvarchar(128)), SUSER_SNAME());

                DECLARE @filters nvarchar(max) =
                    CASE
                        WHEN @ItemIdsJson IS NULL THEN N'{"ItemIds":null}'
                        ELSE (SELECT @ItemIdsJson AS ItemIds FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
                    END;

                -- 실행 헤더: RestoreRun에만 기록 (History는 트리거가 동기화)
                INSERT dbo.RestoreRun (RestoreId, AsOfUtc, CurrentUser, DryRun, Notes, FiltersJson)
                VALUES (@RestoreId, @AsOfUtc, @currentUserName, @DryRun, @Notes, @filters);

                BEGIN TRAN;

                -- 동시 실행 방지 (프로시저 단위)
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
                    RAISERROR('usp_ItemRestore_AsOf: failed to acquire applock (timeout)', 16, 1);
                    RETURN;
                END

                -- 대상 ItemId 집합 (옵션)
                DECLARE @TargetIds TABLE (Id uniqueidentifier NOT NULL PRIMARY KEY);

                IF (@ItemIdsJson IS NOT NULL)
                BEGIN
                    INSERT INTO @TargetIds(Id)
                    SELECT TRY_CAST([value] AS uniqueidentifier)
                    FROM OPENJSON(@ItemIdsJson)
                    WHERE TRY_CAST([value] AS uniqueidentifier) IS NOT NULL;
                END

                -- AsOf 이후 변경 로그 스냅샷
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
                    UPDATE dbo.RestoreRun
                       SET EndedAtUtc = SYSUTCDATETIME(),
                           AffectedCounts = N'{"Delete":0,"Insert":0,"Update":0}'
                     WHERE RestoreId = @RestoreId;

                    COMMIT;

                    SELECT @RestoreId AS RestoreId, 0 AS Deleted, 0 AS Inserted, 0 AS Updated,
                           CAST(0 AS bit) AS IsChanged;
                    RETURN;
                END

                -- 감사 억제 + restore context
                EXEC sys.sp_set_session_context @key=N'audit_skip', @value=1;

                DECLARE @DelApplied TABLE(Id uniqueidentifier PRIMARY KEY);
                DECLARE @InsApplied TABLE(Id uniqueidentifier PRIMARY KEY);
                DECLARE @UpdApplied TABLE(Id uniqueidentifier PRIMARY KEY);

                -- 최신 -> 과거 순으로 역연산
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

                UPDATE dbo.RestoreRun
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

            IF OBJECT_ID(N'dbo.trg_RestoreRun_ToHistory', 'TR') IS NOT NULL
                DROP TRIGGER dbo.trg_RestoreRun_ToHistory;

            IF OBJECT_ID(N'dbo.RestoreRun', 'U') IS NOT NULL
                DROP TABLE dbo.RestoreRun;
            """);
        }
    }
}
