using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTools.Server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceItemInsertWithSafe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF TYPE_ID(N'dbo.ItemInsertTvpType') IS NULL
                    EXEC('CREATE TYPE dbo.ItemInsertTvpType AS TABLE
                    (
                        [Name]        nvarchar(100) NOT NULL,
                        [Price]       int           NOT NULL,
                        [Description] nvarchar(500) NULL,
                        [RarityId]    tinyint       NOT NULL
                    )');
                ");

            // 안전 Insert: 부분성공 + 2번째 결과셋에 StatusCode 반환
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE dbo.Item_Insert_Tvp
                    @Rows dbo.ItemInsertTvpType READONLY
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SET XACT_ABORT ON;

                    -- 유니크 경쟁/팬텀 방지
                    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
                    BEGIN TRAN;

                    -- 1) 입력 + 검증 결과를 물리화
                    DECLARE @Marked TABLE
                    (
                        RowNo         int            NOT NULL PRIMARY KEY,
                        [Name]        nvarchar(100)  NOT NULL,
                        [Price]       int            NOT NULL,
                        [Description] nvarchar(500)  NULL,
                        [RarityId]    tinyint        NOT NULL,
                        StatusCode    int            NOT NULL
                    );

                    INSERT INTO @Marked (RowNo, [Name], [Price], [Description], [RarityId], StatusCode)
                    SELECT
                        ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNo,
                        r.[Name], r.[Price], r.[Description], r.[RarityId],
                        CASE
                            WHEN r.[Price] < 0        THEN 3   -- InvalidPrice (가격은 여기서만 검사)
                            WHEN rar.[Id] IS NULL     THEN 2   -- InvalidRarity
                            WHEN i.[Id] IS NOT NULL   THEN 1   -- DuplicateName (DB/배치)
                            ELSE 0                          -- Insertable
                        END
                    FROM @Rows AS r
                    LEFT JOIN dbo.[Rarity] AS rar ON rar.[Id] = r.[RarityId]
                    LEFT JOIN dbo.[Item]   AS i   WITH (UPDLOCK, HOLDLOCK) ON i.[Name] = r.[Name];

                    -- 2) 성공 행 INSERT + OUTPUT (MERGE를 써서 SOURCE.RowNo 바인딩)
                    DECLARE @Out TABLE
                    (
                        RowNo        int           NOT NULL PRIMARY KEY,
                        [Id]         int           NULL,
                        [RowVersion] varbinary(8)  NULL,
                        StatusCode   int           NOT NULL
                    );

                    MERGE dbo.[Item] AS tgt
                    USING (
                        SELECT RowNo, [Name], [Price], [Description], [RarityId]
                        FROM @Marked
                        WHERE StatusCode = 0
                    ) AS src
                      ON 1 = 0  -- 항상 INSERT
                    WHEN NOT MATCHED THEN
                      INSERT ([Name],[Price],[Description],[RarityId])
                      VALUES (src.[Name], src.[Price], src.[Description], src.[RarityId])
                    OUTPUT src.RowNo, inserted.[Id], inserted.[RowVersion], 0
                      INTO @Out (RowNo, [Id], [RowVersion], StatusCode);

                    -- 3) 실패/스킵 행은 Id/RowVersion = NULL 로 채움
                    INSERT INTO @Out (RowNo, [Id], [RowVersion], StatusCode)
                    SELECT m.RowNo, NULL, NULL, m.StatusCode
                    FROM @Marked AS m
                    WHERE m.StatusCode <> 0;

                    COMMIT;

                    -- 4) 한 결과셋만 반환 (입력 순서대로)
                    SELECT o.[Id], o.[RowVersion], o.[StatusCode]
                    FROM @Out AS o
                    ORDER BY o.RowNo;
                END;
                ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 원래 단순 일괄 삽입 버전으로 복구
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE dbo.Item_Insert_Tvp
                    @Rows dbo.ItemInsertTvpType READONLY
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DECLARE @Inserted TABLE
                    (
                        [Id]         int          NOT NULL,
                        [RowVersion] varbinary(8) NOT NULL
                    );

                    INSERT INTO dbo.[Item] ([Name],[Price],[Description],[RarityId])
                    OUTPUT inserted.[Id], inserted.[RowVersion] INTO @Inserted
                    SELECT r.[Name], r.[Price], r.[Description], r.[RarityId]
                    FROM @Rows AS r;

                    SELECT i.[Id], i.[RowVersion]
                    FROM @Inserted AS i
                    ORDER BY i.[Id];
                END;
                ");
        }
    }
}
