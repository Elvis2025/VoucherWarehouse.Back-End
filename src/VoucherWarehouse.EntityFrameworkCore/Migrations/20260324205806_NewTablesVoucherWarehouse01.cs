using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBS.VoucherWarehouse.Migrations
{
    /// <inheritdoc />
    public partial class NewTablesVoucherWarehouse01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DGIIResponseMessage",
                table: "EcfVoucherWarehouse",
                newName: "DgiiResponseMessage");

            migrationBuilder.RenameColumn(
                name: "TrackId",
                table: "EcfVoucherWarehouse",
                newName: "DgiiTrackId");

            migrationBuilder.AlterColumn<string>(
                name: "DgiiResponseMessage",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DgiiPrintFile",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DgiiQrCodeUrl",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DgiiReceivedDate",
                table: "EcfVoucherWarehouse",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DgiiResponseCode",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DgiiSecurityCode",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DgiiSignatureDate",
                table: "EcfVoucherWarehouse",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DgiiUsedSequence",
                table: "EcfVoucherWarehouse",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DgiiPrintFile",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "DgiiQrCodeUrl",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "DgiiReceivedDate",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "DgiiResponseCode",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "DgiiSecurityCode",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "DgiiSignatureDate",
                table: "EcfVoucherWarehouse");

            migrationBuilder.DropColumn(
                name: "DgiiUsedSequence",
                table: "EcfVoucherWarehouse");

            migrationBuilder.RenameColumn(
                name: "DgiiResponseMessage",
                table: "EcfVoucherWarehouse",
                newName: "DGIIResponseMessage");

            migrationBuilder.RenameColumn(
                name: "DgiiTrackId",
                table: "EcfVoucherWarehouse",
                newName: "TrackId");

            migrationBuilder.AlterColumn<string>(
                name: "DGIIResponseMessage",
                table: "EcfVoucherWarehouse",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);
        }
    }
}
