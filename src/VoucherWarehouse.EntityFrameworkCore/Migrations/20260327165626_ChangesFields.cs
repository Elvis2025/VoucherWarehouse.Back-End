using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBS.VoucherWarehouse.Migrations
{
    /// <inheritdoc />
    public partial class ChangesFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Format",
                table: "TaxVouchersTypes");

            migrationBuilder.DropColumn(
                name: "TaxVoucherLenght",
                table: "TaxVouchersTypes");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "TaxVouchersTypes",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "TaxVouchersTypes",
                newName: "Description");

            migrationBuilder.AddColumn<string>(
                name: "Format",
                table: "TaxVouchersTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaxVoucherLenght",
                table: "TaxVouchersTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
