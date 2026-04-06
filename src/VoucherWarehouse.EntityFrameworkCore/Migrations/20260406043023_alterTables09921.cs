using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBS.VoucherWarehouse.Migrations
{
    /// <inheritdoc />
    public partial class alterTables09921 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConcurrencyStamp",
                table: "EcfVoucherDocumentJobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ExecutionId",
                table: "EcfVoucherDocumentJobs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HeartbeatAt",
                table: "EcfVoucherDocumentJobs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastProgressAt",
                table: "EcfVoucherDocumentJobs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LeaseExpiresAt",
                table: "EcfVoucherDocumentJobs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkerInstanceId",
                table: "EcfVoucherDocumentJobs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "EcfVoucherDocumentJobs");

            migrationBuilder.DropColumn(
                name: "ExecutionId",
                table: "EcfVoucherDocumentJobs");

            migrationBuilder.DropColumn(
                name: "HeartbeatAt",
                table: "EcfVoucherDocumentJobs");

            migrationBuilder.DropColumn(
                name: "LastProgressAt",
                table: "EcfVoucherDocumentJobs");

            migrationBuilder.DropColumn(
                name: "LeaseExpiresAt",
                table: "EcfVoucherDocumentJobs");

            migrationBuilder.DropColumn(
                name: "WorkerInstanceId",
                table: "EcfVoucherDocumentJobs");
        }
    }
}
