using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBS.VoucherWarehouse.Migrations
{
    /// <inheritdoc />
    public partial class ChangesNewTables02123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCancellationRequested",
                table: "EcfVoucherDocumentJobs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCancellationRequested",
                table: "EcfVoucherDocumentJobs");
        }
    }
}
