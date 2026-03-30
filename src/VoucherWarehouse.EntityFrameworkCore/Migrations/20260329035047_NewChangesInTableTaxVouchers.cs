using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBS.VoucherWarehouse.Migrations
{
    /// <inheritdoc />
    public partial class NewChangesInTableTaxVouchers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "TaxVouchers");

            migrationBuilder.DropColumn(
                name: "Prefix",
                table: "TaxVouchers");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "TaxVouchers",
                newName: "Comentario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Comentario",
                table: "TaxVouchers",
                newName: "Description");

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "TaxVouchers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prefix",
                table: "TaxVouchers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
