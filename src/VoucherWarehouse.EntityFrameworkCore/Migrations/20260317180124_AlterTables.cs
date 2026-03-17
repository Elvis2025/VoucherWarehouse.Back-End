using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBS.VoucherWarehouse.Migrations
{
    /// <inheritdoc />
    public partial class AlterTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EcfApiAuthentication",
                table: "EcfApiAuthentication");

            migrationBuilder.RenameTable(
                name: "EcfApiAuthentication",
                newName: "EcfApiAuthentications");

            migrationBuilder.RenameIndex(
                name: "IX_EcfApiAuthentication_TenantId",
                table: "EcfApiAuthentications",
                newName: "IX_EcfApiAuthentications_TenantId");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TaxVouchersTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "TaxVouchersTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TaxVouchers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "TaxVouchers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "EcfVoucherWarehouses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "EcfVoucherWarehouses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "EcfApiAuthentications",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "EcfApiAuthentications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EcfApiAuthentications",
                table: "EcfApiAuthentications",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EcfApiAuthentications",
                table: "EcfApiAuthentications");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TaxVouchersTypes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TaxVouchersTypes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TaxVouchers");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TaxVouchers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "EcfVoucherWarehouses");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "EcfVoucherWarehouses");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "EcfApiAuthentications");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "EcfApiAuthentications");

            migrationBuilder.RenameTable(
                name: "EcfApiAuthentications",
                newName: "EcfApiAuthentication");

            migrationBuilder.RenameIndex(
                name: "IX_EcfApiAuthentications_TenantId",
                table: "EcfApiAuthentication",
                newName: "IX_EcfApiAuthentication_TenantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EcfApiAuthentication",
                table: "EcfApiAuthentication",
                column: "Id");
        }
    }
}
