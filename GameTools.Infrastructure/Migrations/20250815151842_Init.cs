using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTools.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemAudit",
                columns: table => new
                {
                    AuditId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BeforeJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AfterJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    ChangedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false, defaultValue: "system")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAudit", x => x.AuditId);
                    table.CheckConstraint("CK_ItemAudit_Action", "UPPER([Action]) IN ('INSERT','UPDATE','DELETE')");
                    table.CheckConstraint("CK_ItemAudit_After_IsJson", "([AfterJson]  IS NULL OR ISJSON([AfterJson])  = 1)");
                    table.CheckConstraint("CK_ItemAudit_Before_IsJson", "([BeforeJson] IS NULL OR ISJSON([BeforeJson]) = 1)");
                });

            migrationBuilder.CreateTable(
                name: "Rarity",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Grade = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ColorCode = table.Column<string>(type: "char(7)", nullable: false, defaultValue: "#A0A0A0"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rarity", x => x.Id);
                    table.CheckConstraint("CK_Rarity_ColorCode_Format", "LEN([ColorCode]) = 7 AND LEFT([ColorCode],1) = '#' AND [ColorCode] = UPPER([ColorCode]) AND [ColorCode] LIKE '#[0-9A-F][0-9A-F][0-9A-F][0-9A-F][0-9A-F][0-9A-F]'");
                });

            migrationBuilder.CreateTable(
                name: "RarityAudit",
                columns: table => new
                {
                    AuditId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RarityId = table.Column<byte>(type: "tinyint", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BeforeJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AfterJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    ChangedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false, defaultValue: "system")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RarityAudit", x => x.AuditId);
                    table.CheckConstraint("CK_RarityAudit_Action", "UPPER([Action]) IN ('INSERT','UPDATE','DELETE')");
                    table.CheckConstraint("CK_RarityAudit_After_IsJson", "([AfterJson]  IS NULL OR ISJSON([AfterJson])  = 1)");
                    table.CheckConstraint("CK_RarityAudit_Before_IsJson", "([BeforeJson] IS NULL OR ISJSON([BeforeJson]) = 1)");
                });

            migrationBuilder.CreateTable(
                name: "Item",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RarityId = table.Column<byte>(type: "tinyint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item", x => x.Id);
                    table.CheckConstraint("CK_Item_Price_NonNegative", "[Price] >= 0");
                    table.ForeignKey(
                        name: "FK_Item_Rarity_RarityId",
                        column: x => x.RarityId,
                        principalTable: "Rarity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Item_Name",
                table: "Item",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Item_RarityId",
                table: "Item",
                column: "RarityId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAudit_ChangedAtUtc",
                table: "ItemAudit",
                column: "ChangedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAudit_ItemId_ChangedAtUtc",
                table: "ItemAudit",
                columns: new[] { "ItemId", "ChangedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Rarity_ColorCode",
                table: "Rarity",
                column: "ColorCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rarity_Grade",
                table: "Rarity",
                column: "Grade",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RarityAudit_ChangedAtUtc",
                table: "RarityAudit",
                column: "ChangedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_RarityAudit_RarityId_ChangedAtUtc",
                table: "RarityAudit",
                columns: new[] { "RarityId", "ChangedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Item");

            migrationBuilder.DropTable(
                name: "ItemAudit");

            migrationBuilder.DropTable(
                name: "RarityAudit");

            migrationBuilder.DropTable(
                name: "Rarity");
        }
    }
}
