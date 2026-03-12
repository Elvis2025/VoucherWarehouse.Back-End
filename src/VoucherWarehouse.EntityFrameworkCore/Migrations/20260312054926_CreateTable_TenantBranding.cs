using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBS.VoucherWarehouse.Migrations;

/// <inheritdoc />
public partial class CreateTable_TenantBranding : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "TenantBrandings",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TenantId = table.Column<int>(type: "int", nullable: false),
                LogoPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                LogoFileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                LogoContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                CompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                CompanyType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                CompanyDescription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                LogoSize = table.Column<long>(type: "bigint", nullable: true),
                CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TenantBrandings", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "TenantBrandings");
    }
}
