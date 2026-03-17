using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBS.VoucherWarehouse.Migrations
{
    /// <inheritdoc />
    public partial class AlterTableEcfVoucherWarehouseChangeTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EcfVoucherWarehouse",
                table: "EcfVoucherWarehouse");

            migrationBuilder.RenameTable(
                name: "EcfVoucherWarehouse",
                newName: "EcfVoucherWarehouses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EcfVoucherWarehouses",
                table: "EcfVoucherWarehouses",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EcfVoucherWarehouses",
                table: "EcfVoucherWarehouses");

            migrationBuilder.RenameTable(
                name: "EcfVoucherWarehouses",
                newName: "EcfVoucherWarehouse");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EcfVoucherWarehouse",
                table: "EcfVoucherWarehouse",
                column: "Id");
        }
    }
}
