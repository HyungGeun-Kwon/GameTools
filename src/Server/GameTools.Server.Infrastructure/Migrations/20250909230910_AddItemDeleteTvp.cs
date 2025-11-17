using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTools.Server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddItemDeleteTvp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Delete TVP
            migrationBuilder.Sql(@"
                IF TYPE_ID(N'dbo.ItemDeleteTvpType') IS NULL
                    EXEC('CREATE TYPE dbo.ItemDeleteTvpType AS TABLE
                    (
                        [Id]                 int          NOT NULL,
                        [RowVersionOriginal] varbinary(8) NOT NULL,
                        PRIMARY KEY NONCLUSTERED ([Id])
                    )');
            ");

            // Delete SP
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE dbo.Item_Delete_Tvp
                    @Rows dbo.ItemDeleteTvpType READONLY
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SET XACT_ABORT ON;

                    -- 보수적 격리로 팬텀/경쟁 최소화
                    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
                    BEGIN TRAN;

                    -- 결과 버퍼
                    DECLARE @Out TABLE
                    (
                        [Id]        int NOT NULL PRIMARY KEY,
                        StatusCode  int NOT NULL
                    );

                    -- 1) (Id, RowVersion) 일치 행 삭제 + 성공 기록
                    DELETE i
                    OUTPUT deleted.[Id], 0
                    INTO @Out ([Id], StatusCode)
                    FROM dbo.[Item] AS i WITH (UPDLOCK, HOLDLOCK)
                    INNER JOIN @Rows AS r
                      ON r.[Id] = i.[Id]
                     AND i.[RowVersion] = r.[RowVersionOriginal];

                    -- 2) 삭제되지 않은 입력 행에 대해 NotFound/ConcurrencyMismatch 채우기
                    INSERT INTO @Out ([Id], StatusCode)
                    SELECT r.[Id],
                           CASE WHEN i.[Id] IS NULL THEN 1 ELSE 2 END
                    FROM @Rows AS r
                    LEFT JOIN dbo.[Item] AS i WITH (HOLDLOCK) ON i.[Id] = r.[Id]
                    WHERE NOT EXISTS (SELECT 1 FROM @Out o WHERE o.[Id] = r.[Id]);

                    COMMIT;

                    -- Id 기준 정렬로 반환
                    SELECT [Id], StatusCode
                    FROM @Out
                    ORDER BY [Id];
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF OBJECT_ID(N'dbo.Item_Delete_Tvp', N'P') IS NOT NULL DROP PROCEDURE dbo.Item_Delete_Tvp;");
            migrationBuilder.Sql(@"IF TYPE_ID(N'dbo.ItemDeleteTvpType') IS NOT NULL DROP TYPE dbo.ItemDeleteTvpType;");
        }
    }
}
