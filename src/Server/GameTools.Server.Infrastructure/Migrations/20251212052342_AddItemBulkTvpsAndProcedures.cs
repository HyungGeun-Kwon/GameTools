using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTools.Server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddItemBulkTvpsAndProcedures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) TVP Types -------------------------------------------------------

            migrationBuilder.Sql(
            """
        IF TYPE_ID(N'dbo.ItemInsertTvp') IS NULL
        EXEC(N'
        CREATE TYPE dbo.ItemInsertTvp AS TABLE
        (
            [Index]     int            NOT NULL,
            Name        nvarchar(100)  NOT NULL,
            Price       int            NOT NULL,
            Description nvarchar(1000) NULL,
            RarityId    uniqueidentifier NOT NULL
        );');
        """);

            migrationBuilder.Sql(
            """
        IF TYPE_ID(N'dbo.ItemUpdateTvp') IS NULL
        EXEC(N'
        CREATE TYPE dbo.ItemUpdateTvp AS TABLE
        (
            [Index]            int              NOT NULL,
            Id                 uniqueidentifier NOT NULL,
            Name               nvarchar(100)    NOT NULL,
            Price              int              NOT NULL,
            Description        nvarchar(1000)   NULL,
            RarityId           uniqueidentifier NOT NULL,
            RowVersionOriginal varbinary(8)     NOT NULL
        );');
        """);

            migrationBuilder.Sql(
            """
        IF TYPE_ID(N'dbo.ItemDeleteTvp') IS NULL
        EXEC(N'
        CREATE TYPE dbo.ItemDeleteTvp AS TABLE
        (
            [Index]            int              NOT NULL,
            Id                 uniqueidentifier NOT NULL,
            RowVersionOriginal varbinary(8)     NOT NULL
        );');
        """);

            // 2) Bulk Insert SP --------------------------------------------------

            migrationBuilder.Sql(
            """
        CREATE OR ALTER PROCEDURE dbo.usp_ItemBulkInsert
            @Items dbo.ItemInsertTvp READONLY
        AS
        BEGIN
            SET NOCOUNT ON;

            BEGIN TRY
                DECLARE @R TABLE
                (
                    [Index]      int              NOT NULL,
                    Id           uniqueidentifier NOT NULL,
                    RowVersion   varbinary(8)     NULL,
                    [Status]     tinyint          NOT NULL,
                    ErrorCode    nvarchar(50)     NULL,
                    ErrorMessage nvarchar(4000)   NULL
                );

                -- Work set: generate ids once (NEWSEQUENTIALID)
                DECLARE @W TABLE
                (
                    [Index]     int              NOT NULL PRIMARY KEY,
                    Id          uniqueidentifier NOT NULL,
                    Name        nvarchar(100)    NOT NULL,
                    Price       int              NOT NULL,
                    Description nvarchar(1000)   NULL,
                    RarityId    uniqueidentifier NOT NULL
                );

                INSERT INTO @W([Index], Id, Name, Price, Description, RarityId)
                SELECT i.[Index], NEWSEQUENTIALID(), i.Name, i.Price, i.Description, i.RarityId
                FROM @Items i;

                ;WITH BatchNameDup AS
                (
                    SELECT [Index]
                    FROM
                    (
                        SELECT [Index], Name,
                               COUNT(*) OVER (PARTITION BY Name) AS Cnt,
                               ROW_NUMBER() OVER (PARTITION BY Name ORDER BY [Index]) AS Rn
                        FROM @W
                    ) d
                    WHERE d.Cnt > 1 AND d.Rn > 1
                ),
                ExistingNameDup AS
                (
                    SELECT w.[Index]
                    FROM @W w
                    JOIN dbo.Item t ON t.Name = w.Name
                ),
                MissingRarity AS
                (
                    SELECT w.[Index]
                    FROM @W w
                    LEFT JOIN dbo.Rarity r ON r.Id = w.RarityId
                    WHERE r.Id IS NULL
                ),
                BadPrice AS
                (
                    SELECT w.[Index]
                    FROM @W w
                    WHERE w.Price < 0
                )
                INSERT INTO @R([Index], Id, RowVersion, [Status], ErrorCode, ErrorMessage)
                SELECT
                    w.[Index],
                    CASE
                        WHEN bp.[Index] IS NOT NULL THEN CONVERT(uniqueidentifier, 0x00000000000000000000000000000000)
                        WHEN mr.[Index] IS NOT NULL THEN CONVERT(uniqueidentifier, 0x00000000000000000000000000000000)
                        WHEN en.[Index] IS NOT NULL THEN CONVERT(uniqueidentifier, 0x00000000000000000000000000000000)
                        WHEN bd.[Index] IS NOT NULL THEN CONVERT(uniqueidentifier, 0x00000000000000000000000000000000)
                        ELSE w.Id
                    END,
                    NULL,
                    CASE
                        WHEN bp.[Index] IS NOT NULL THEN 3  -- ValidationFailed
                        WHEN mr.[Index] IS NOT NULL THEN 3
                        WHEN en.[Index] IS NOT NULL THEN 4  -- Conflict
                        WHEN bd.[Index] IS NOT NULL THEN 4
                        ELSE 0                               -- Succeeded (pending insert)
                    END,
                    CASE
                        WHEN bp.[Index] IS NOT NULL THEN N'InvalidPrice'
                        WHEN mr.[Index] IS NOT NULL THEN N'InvalidRarity'
                        WHEN en.[Index] IS NOT NULL OR bd.[Index] IS NOT NULL THEN N'DuplicateName'
                        ELSE NULL
                    END,
                    NULL
                FROM @W w
                LEFT JOIN BadPrice bp        ON bp.[Index] = w.[Index]
                LEFT JOIN MissingRarity mr   ON mr.[Index] = w.[Index]
                LEFT JOIN ExistingNameDup en ON en.[Index] = w.[Index]
                LEFT JOIN BatchNameDup bd    ON bd.[Index] = w.[Index];

                -- Insert only rows that are marked Succeeded(0) AND have non-empty Id
                DECLARE @Inserted TABLE([Index] int NOT NULL PRIMARY KEY, Id uniqueidentifier NOT NULL, RowVersion varbinary(8) NOT NULL);

                INSERT INTO dbo.Item (Id, Name, Price, Description, RarityId)
                OUTPUT w.[Index], inserted.Id, inserted.RowVersion
                INTO @Inserted([Index], Id, RowVersion)
                SELECT w.Id, w.Name, w.Price, w.Description, w.RarityId
                FROM @W w
                JOIN @R r ON r.[Index] = w.[Index]
                WHERE r.[Status] = 0 AND r.Id <> CONVERT(uniqueidentifier, 0x00000000000000000000000000000000);

                -- Fill RowVersion for successes
                UPDATE r
                SET r.RowVersion = ins.RowVersion
                FROM @R r
                JOIN @Inserted ins ON ins.[Index] = r.[Index];

                SELECT [Index], Id, RowVersion, [Status], ErrorCode, ErrorMessage
                FROM @R
                ORDER BY [Index];
            END TRY
            BEGIN CATCH
                -- Catastrophic error: return UnknownError for all input rows
                SELECT
                    i.[Index],
                    CONVERT(uniqueidentifier, 0x00000000000000000000000000000000) AS Id,
                    CAST(NULL AS varbinary(8)) AS RowVersion,
                    CAST(5 AS tinyint) AS [Status],
                    CONVERT(nvarchar(50), ERROR_NUMBER()) AS ErrorCode,
                    ERROR_MESSAGE() AS ErrorMessage
                FROM @Items i
                ORDER BY i.[Index];
            END CATCH
        END;
        """);

            // 3) Bulk Update SP --------------------------------------------------

            migrationBuilder.Sql(
            """
        CREATE OR ALTER PROCEDURE dbo.usp_ItemBulkUpdate
            @Items dbo.ItemUpdateTvp READONLY
        AS
        BEGIN
            SET NOCOUNT ON;

            BEGIN TRY
                DECLARE @R TABLE
                (
                    [Index]      int              NOT NULL PRIMARY KEY,
                    Id           uniqueidentifier NOT NULL,
                    RowVersion   varbinary(8)     NULL,
                    [Status]     tinyint          NOT NULL,
                    ErrorCode    nvarchar(50)     NULL,
                    ErrorMessage nvarchar(4000)   NULL
                );

                -- Base classification: NotFound / Concurrency / Validation / Conflict / SuccessCandidate
                ;WITH T AS
                (
                    SELECT
                        s.[Index], s.Id, s.Name, s.Price, s.Description, s.RarityId, s.RowVersionOriginal,
                        t.Id AS ExistingId,
                        t.RowVersion AS ExistingRowVersion
                    FROM @Items s
                    LEFT JOIN dbo.Item t ON t.Id = s.Id
                ),
                MissingRarity AS
                (
                    SELECT t.[Index]
                    FROM T t
                    LEFT JOIN dbo.Rarity r ON r.Id = t.RarityId
                    WHERE t.ExistingId IS NOT NULL AND r.Id IS NULL
                ),
                BadPrice AS
                (
                    SELECT [Index]
                    FROM T
                    WHERE ExistingId IS NOT NULL AND Price < 0
                ),
                NameConflict AS
                (
                    -- conflict with other rows in DB (same name, different Id)
                    SELECT t.[Index]
                    FROM T t
                    JOIN dbo.Item x ON x.Name = t.Name AND x.Id <> t.Id
                    WHERE t.ExistingId IS NOT NULL
                ),
                BatchNameDup AS
                (
                    -- duplicates inside batch (same name for different ids) -> mark 2nd+ as conflict
                    SELECT [Index]
                    FROM
                    (
                        SELECT [Index], Name,
                               COUNT(*) OVER (PARTITION BY Name) AS Cnt,
                               ROW_NUMBER() OVER (PARTITION BY Name ORDER BY [Index]) AS Rn
                        FROM T
                        WHERE ExistingId IS NOT NULL
                    ) d
                    WHERE d.Cnt > 1 AND d.Rn > 1
                )
                INSERT INTO @R([Index], Id, RowVersion, [Status], ErrorCode, ErrorMessage)
                SELECT
                    t.[Index],
                    t.Id,
                    NULL,
                    CASE
                        WHEN t.ExistingId IS NULL THEN 1 -- NotFound
                        WHEN t.ExistingRowVersion <> t.RowVersionOriginal THEN 2 -- Concurrency
                        WHEN bp.[Index] IS NOT NULL THEN 3
                        WHEN mr.[Index] IS NOT NULL THEN 3
                        WHEN nc.[Index] IS NOT NULL OR bd.[Index] IS NOT NULL THEN 4
                        ELSE 0
                    END,
                    CASE
                        WHEN t.ExistingId IS NULL THEN N'NotFound'
                        WHEN t.ExistingRowVersion <> t.RowVersionOriginal THEN N'Concurrency'
                        WHEN bp.[Index] IS NOT NULL THEN N'InvalidPrice'
                        WHEN mr.[Index] IS NOT NULL THEN N'InvalidRarity'
                        WHEN nc.[Index] IS NOT NULL OR bd.[Index] IS NOT NULL THEN N'DuplicateName'
                        ELSE NULL
                    END,
                    NULL
                FROM T t
                LEFT JOIN BadPrice bp      ON bp.[Index] = t.[Index]
                LEFT JOIN MissingRarity mr ON mr.[Index] = t.[Index]
                LEFT JOIN NameConflict nc  ON nc.[Index] = t.[Index]
                LEFT JOIN BatchNameDup bd  ON bd.[Index] = t.[Index];

                -- Execute updates for candidates
                DECLARE @Updated TABLE([Index] int NOT NULL PRIMARY KEY, RowVersion varbinary(8) NOT NULL);

                UPDATE tgt
                SET
                    tgt.Name = src.Name,
                    tgt.Price = src.Price,
                    tgt.Description = src.Description,
                    tgt.RarityId = src.RarityId
                OUTPUT src.[Index], inserted.RowVersion
                INTO @Updated([Index], RowVersion)
                FROM dbo.Item tgt
                JOIN @Items src ON src.Id = tgt.Id
                JOIN @R r ON r.[Index] = src.[Index]
                WHERE r.[Status] = 0 AND tgt.RowVersion = src.RowVersionOriginal;

                UPDATE r
                SET r.RowVersion = u.RowVersion
                FROM @R r
                JOIN @Updated u ON u.[Index] = r.[Index];

                SELECT [Index], Id, RowVersion, [Status], ErrorCode, ErrorMessage
                FROM @R
                ORDER BY [Index];
            END TRY
            BEGIN CATCH
                SELECT
                    i.[Index],
                    i.Id,
                    CAST(NULL AS varbinary(8)) AS RowVersion,
                    CAST(5 AS tinyint) AS [Status],
                    CONVERT(nvarchar(50), ERROR_NUMBER()) AS ErrorCode,
                    ERROR_MESSAGE() AS ErrorMessage
                FROM @Items i
                ORDER BY i.[Index];
            END CATCH
        END;
        """);

            // 4) Bulk Delete SP --------------------------------------------------

            migrationBuilder.Sql(
            """
        CREATE OR ALTER PROCEDURE dbo.usp_ItemBulkDelete
            @Items dbo.ItemDeleteTvp READONLY
        AS
        BEGIN
            SET NOCOUNT ON;

            BEGIN TRY
                DECLARE @R TABLE
                (
                    [Index]      int              NOT NULL PRIMARY KEY,
                    Id           uniqueidentifier NOT NULL,
                    RowVersion   varbinary(8)     NULL,
                    [Status]     tinyint          NOT NULL,
                    ErrorCode    nvarchar(50)     NULL,
                    ErrorMessage nvarchar(4000)   NULL
                );

                ;WITH T AS
                (
                    SELECT
                        s.[Index], s.Id, s.RowVersionOriginal,
                        t.Id AS ExistingId,
                        t.RowVersion AS ExistingRowVersion
                    FROM @Items s
                    LEFT JOIN dbo.Item t ON t.Id = s.Id
                )
                INSERT INTO @R([Index], Id, RowVersion, [Status], ErrorCode, ErrorMessage)
                SELECT
                    t.[Index],
                    t.Id,
                    NULL,
                    CASE
                        WHEN t.ExistingId IS NULL THEN 1
                        WHEN t.ExistingRowVersion <> t.RowVersionOriginal THEN 2
                        ELSE 0
                    END,
                    CASE
                        WHEN t.ExistingId IS NULL THEN N'NotFound'
                        WHEN t.ExistingRowVersion <> t.RowVersionOriginal THEN N'Concurrency'
                        ELSE NULL
                    END,
                    NULL
                FROM T t;

                -- Delete only candidates
                DELETE tgt
                FROM dbo.Item tgt
                JOIN @Items src ON src.Id = tgt.Id
                JOIN @R r ON r.[Index] = src.[Index]
                WHERE r.[Status] = 0 AND tgt.RowVersion = src.RowVersionOriginal;

                SELECT [Index], Id, RowVersion, [Status], ErrorCode, ErrorMessage
                FROM @R
                ORDER BY [Index];
            END TRY
            BEGIN CATCH
                SELECT
                    i.[Index],
                    i.Id,
                    CAST(NULL AS varbinary(8)) AS RowVersion,
                    CAST(5 AS tinyint) AS [Status],
                    CONVERT(nvarchar(50), ERROR_NUMBER()) AS ErrorCode,
                    ERROR_MESSAGE() AS ErrorMessage
                FROM @Items i
                ORDER BY i.[Index];
            END CATCH
        END;
        """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
            """
        IF OBJECT_ID(N'dbo.usp_ItemBulkInsert', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_ItemBulkInsert;
        IF OBJECT_ID(N'dbo.usp_ItemBulkUpdate', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_ItemBulkUpdate;
        IF OBJECT_ID(N'dbo.usp_ItemBulkDelete', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_ItemBulkDelete;

        IF TYPE_ID(N'dbo.ItemInsertTvp') IS NOT NULL DROP TYPE dbo.ItemInsertTvp;
        IF TYPE_ID(N'dbo.ItemUpdateTvp') IS NOT NULL DROP TYPE dbo.ItemUpdateTvp;
        IF TYPE_ID(N'dbo.ItemDeleteTvp') IS NOT NULL DROP TYPE dbo.ItemDeleteTvp;
        """);
        }
    }
}
