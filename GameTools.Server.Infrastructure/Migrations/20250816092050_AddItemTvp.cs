using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTools.Server.Infrastructure.Migrations
{
    public partial class AddItemTvp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert TVP (필요시 PK 생략 가능)
            migrationBuilder.Sql(@"
                IF TYPE_ID(N'dbo.ItemInsertTvpType') IS NULL
                    EXEC('CREATE TYPE dbo.ItemInsertTvpType AS TABLE
                    (
                        [Name]        nvarchar(100) NOT NULL,
                        [Price]       int           NOT NULL,
                        [Description] nvarchar(500) NULL,
                        [RarityId]    tinyint       NOT NULL
                    )');");

            // Update TVP (PK로 중복/성능 보호)
            migrationBuilder.Sql(@"
                IF TYPE_ID(N'dbo.ItemUpdateTvpType') IS NULL
                    EXEC('CREATE TYPE dbo.ItemUpdateTvpType AS TABLE
                    (
                        [Id]                 int            NOT NULL,
                        [Name]               nvarchar(100)  NOT NULL,
                        [Price]              int            NOT NULL,
                        [Description]        nvarchar(500)  NULL,
                        [RarityId]           tinyint        NOT NULL,
                        [RowVersionOriginal] varbinary(8)   NOT NULL,
                        PRIMARY KEY NONCLUSTERED ([Id])
                    )');");

            // Insert SP
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

            // Update SP
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE dbo.Item_Update_Tvp
                    @Rows dbo.ItemUpdateTvpType READONLY
                AS
                BEGIN
                    SET NOCOUNT ON;

                    -- 먼저 조건(RowVersion 일치) 충족한 행만 갱신하고 새 RowVersion을 수집
                    DECLARE @Updated TABLE
                    (
                        [Id]         int          NOT NULL,
                        [RowVersion] varbinary(8) NOT NULL,
                        PRIMARY KEY ([Id])
                    );

                    UPDATE i
                        SET i.[Name]        = r.[Name],
                            i.[Price]       = r.[Price],
                            i.[Description] = r.[Description],
                            i.[RarityId]    = r.[RarityId]
                    OUTPUT inserted.[Id], inserted.[RowVersion]
                    INTO @Updated([Id],[RowVersion])
                    FROM dbo.[Item] AS i
                    INNER JOIN @Rows AS r ON r.[Id] = i.[Id]
                    WHERE i.[RowVersion] = r.[RowVersionOriginal];

                    ;WITH ExistsCheck AS
                    (
                        SELECT r.[Id], i.[RowVersion] AS CurrentRowVersion
                        FROM @Rows r
                        LEFT JOIN dbo.[Item] i ON i.[Id] = r.[Id]
                    )
                    SELECT
                        r.[Id],
                        u.[RowVersion] AS NewRowVersion,
                        CASE
                            WHEN u.[Id] IS NOT NULL THEN 0          -- Updated
                            WHEN e.CurrentRowVersion IS NULL THEN 1 -- NotFound
                            ELSE 2                                  -- Concurrency
                        END AS StatusCode
                    FROM @Rows r
                    LEFT JOIN @Updated u ON u.[Id] = r.[Id]
                    LEFT JOIN ExistsCheck e ON e.[Id] = r.[Id]
                    ORDER BY r.[Id];
                END;
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF OBJECT_ID(N'dbo.Item_Update_Tvp', N'P') IS NOT NULL DROP PROCEDURE dbo.Item_Update_Tvp;");
            migrationBuilder.Sql(@"IF OBJECT_ID(N'dbo.Item_Insert_Tvp', N'P') IS NOT NULL DROP PROCEDURE dbo.Item_Insert_Tvp;");
            migrationBuilder.Sql(@"IF TYPE_ID(N'dbo.ItemUpdateTvpType') IS NOT NULL DROP TYPE dbo.ItemUpdateTvpType;");
            migrationBuilder.Sql(@"IF TYPE_ID(N'dbo.ItemInsertTvpType') IS NOT NULL DROP TYPE dbo.ItemInsertTvpType;");
        }
    }
}
